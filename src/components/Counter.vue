<script setup>
  import { reactive, ref, onMounted, nextTick } from "vue";
  const props = defineProps(['threshold'])
  const emit = defineEmits('onDetected')

  const state = reactive({
    isLoading: false,
  });

  const camera = ref(null)
  const canvas = ref(null)
  const overlay = ref(null)
  let model = null
  onMounted(async () => {
    state.isLoading = true;
    model = await loadModel();
    await openCamera();
    state.isLoading = false;
  });

  const openCamera = async () => {
    const openMediaDevices = async (constraints) => {
      return await navigator.mediaDevices.getUserMedia(constraints);
    };

    try {
      const stream = await openMediaDevices({ video: true, audio: false });
      camera.value.srcObject = stream;
    } catch (error) {
      console.error("Error accessing camera.", error);
    }

    const draw = () => {
      const context = canvas.value.getContext("2d");
      // context.scale(-1, 1)
      context.drawImage(camera.value, 0, 0, camera.value.videoWidth,camera.value.videoHeight);
      window.requestAnimationFrame(draw);
    };

    window.requestAnimationFrame(draw);
  };

  const updateCanvas = () => {
    canvas.value.width = camera.value.videoWidth
    canvas.value.height = camera.value.videoHeight
    overlay.value.width = camera.value.videoWidth
    overlay.value.height = camera.value.videoHeight
  }
  
let loadModel = async () => {
  let model = new cvstfjs.ObjectDetectionModel();
  await model.loadModelAsync("/v4/model.json");
  return model;
};

async function checkout() {
  let tensor = tf.browser
    .fromPixels(camera.value, 3)
    .resizeNearestNeighbor([416, 416]) // change the image size
    .expandDims()
    .toFloat()
    .reverse(-1); // RGB -> BGR

  let result = zip(...(await model.executeAsync(tensor)))
  let ctx = overlay.value.getContext("2d")
  ctx.reset()
  const context = canvas.value.getContext("2d");
  let data = result
  .filter(r => r[1] > props.threshold)
  .map(r => {
    let bboxLeft = r[0][0] * camera.value.videoWidth;
    let bboxTop = r[0][1] * camera.value.videoHeight;
    let bboxWidth = r[0][2] * camera.value.videoWidth - bboxLeft;
    let bboxHeight = r[0][3] * camera.value.videoHeight - bboxTop;
    ctx.rect(bboxLeft, bboxTop, bboxWidth, bboxHeight);
    ctx.strokeStyle = "#0000ff";
    ctx.lineWidth = 3;
    let imageData = context.getImageData(bboxLeft, bboxTop, bboxWidth, bboxHeight)
    return  {
      rect: r[0], 
      sku: r[2], 
      confidence: r[1],
      imageData: imageData
    }
  })
  ctx.stroke()
  emit("onDetected",data)
}

const zip = (arr, ...arrs) => {
  return arr.map((val, i) => arrs.reduce((a, arr) => [...a, arr[i]], [val]));
};
</script>

<template>
  <section class="content">
    <button type="button" class="button" @click="checkout">
      <img
        src="https://img.icons8.com/material-outlined/50/000000/camera--v2.png"
      />
    </button>
    <video
      ref="camera"
      id="camera"
      @resize="updateCanvas"
      autoplay
    ></video>
    <canvas ref="canvas" id="canvas" ></canvas>
    <canvas ref="overlay" id="canvas" ></canvas>
  </section>
</template>

<style scoped>
  .content {
    margin-top:10px;
    position: relative;
    flex: auto;
  }
  video {
    display: none;
    position: absolute;
    top: 0;
    left: 0;
  }
  canvas {
    background:transparent;
    position: absolute;
    top: 0;
    left: 0;
  }
  
  button {
    position: fixed;
    float: right;
    z-index: 999;
  }
</style>