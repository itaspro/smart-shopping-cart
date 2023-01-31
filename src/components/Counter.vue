<script setup>
  import { reactive, ref, onMounted, nextTick } from "vue";
  const props = defineProps(['onDetected'])
  const state = reactive({
    isLoading: false,
  });

  const camera = ref(null)
  const canvas = ref(null)

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
      context.scale(-1, 1)
      context.drawImage(camera.value, 0, 0, camera.value.videoWidth,camera.value.videoHeight);
      window.requestAnimationFrame(draw);
    };

    window.requestAnimationFrame(draw);
  };

  const updateCanvas = () => {
    canvas.value.width = camera.value.videoWidth
    canvas.value.height = camera.value.videoHeight
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

  let data = await model.executeAsync(tensor);
  props.onDetected(data)
}

</script>

<template>
  <div class="content">
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
  </div>
</template>

<style scoped>

  video {
    display: none;
  }
  canvas {
    background:transparent;
  }
  
  button {
    position: fixed;
    float: right;
    z-index: 999;
  }
</style>