<script setup>
import * as cvstfjs from "@microsoft/customvision-tfjs";
import * as tf from "@tensorflow/tfjs";
import { reactive, ref, onMounted, nextTick } from "vue";
import ProductItem from "./ProductItem.vue";
import AddProductItem from "./AddProductItem.vue"

const camera = ref(null);
const canvas = ref(null);
const overlay = ref(null);

let ctx = null;
let rect = {};
let drag = false;
let model = null;
const props = defineProps(["threshold", "labels"]);
const emit = defineEmits(["onDetected", "onProductItemAdded"]);
const state = reactive({
  imageData: null,
  selectedArea: {with: 0, hieght: 0},
  isLoading: false,
  dialog: false,
});

onMounted(async () => {
  state.isLoading = true;
  model = await loadModel();
  await openCamera();
  setupCanvas();
  state.isLoading = false;

  window.addEventListener("keydown", e => {
    if (e.key=='Enter') {
      scan();
    }
  });
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
    if (canvas.value) {
      const context = canvas.value.getContext("2d");
      context.drawImage(
        camera.value,
        0,
        0,
        camera.value.videoWidth,
        camera.value.videoHeight
      );
    }
    window.requestAnimationFrame(draw);
  };

  window.requestAnimationFrame(draw);
};
const setupCanvas = () => {
  overlay.value.addEventListener("mousedown", mouseDown, false);
  overlay.value.addEventListener("mouseup", mouseUp, false);
  overlay.value.addEventListener("mousemove", mouseMove, false);
  ctx = overlay.value.getContext("2d");
};

  const mouseDown = (e) => {
    rect.startX = e.offsetX;
    rect.startY = e.offsetY;
    drag = true;
  };

  const mouseUp = () => {
    drag = false;
    if (rect.w < 10) {
      return;
    }
    let ctx = canvas.value.getContext("2d");
    state.imageData = ctx.getImageData(
      Math.round(rect.startX),
      Math.round(rect.startY),
      Math.round(rect.w),
      Math.round(rect.h)
    );
    state.selectedArea = {width: rect.w, height: rect.h}
    overlay.value.getContext("2d").reset();
    state.dialog = true && rect.w > 10 
    rect.w = 0;
    rect.h = 0;
  };
  const mouseMove = (e) => {
    if (drag) {
      rect.w = e.offsetX - rect.startX;
      rect.h = e.offsetY - rect.startY;
      if (rect.w > 10 && rect.h > 10) {
        ctx.clearRect(0, 0, overlay.value.width, overlay.value.height);
        ctx.setLineDash([6]);
        ctx.strokeRect(rect.startX, rect.startY, rect.w, rect.h);
      }
    }
  };

const updateCanvas = () => {
  canvas.value.width = camera.value.videoWidth;
  canvas.value.height = camera.value.videoHeight;
  overlay.value.width = camera.value.videoWidth;
  overlay.value.height = camera.value.videoHeight;
};

let loadModel = async () => {
  let model = new cvstfjs.ObjectDetectionModel();
  await model.loadModelAsync("/v4/model.json");
  return model;
};

async function scan() {
  state.isLoading = true
  let tensor = tf.browser
    .fromPixels(camera.value, 3)
    .resizeNearestNeighbor([416, 416]) // change the image size
    .expandDims()
    .toFloat()
    .reverse(-1); // RGB -> BGR

  let result = zip(...(await model.executeAsync(tensor)));
  let ctx = overlay.value.getContext("2d");
  ctx.reset();
  const context = canvas.value.getContext("2d");
  let data = result
    .filter((r) => r[1] > props.threshold)
    .map((r) => {
      let bboxLeft = r[0][0] * camera.value.videoWidth;
      let bboxTop = r[0][1] * camera.value.videoHeight;
      let bboxWidth = r[0][2] * camera.value.videoWidth - bboxLeft;
      let bboxHeight = r[0][3] * camera.value.videoHeight - bboxTop;
      ctx.rect(bboxLeft, bboxTop, bboxWidth, bboxHeight);
      ctx.strokeStyle = "#0000ff";
      ctx.lineWidth = 3;
      let imageData = context.getImageData(
        bboxLeft,
        bboxTop,
        bboxWidth,
        bboxHeight
      );
      return {
        rect: r[0],
        sku: r[2],
        confidence: r[1],
        imageData: imageData,
      };
    });
  ctx.stroke();
  emit("onDetected", data);
  state.isLoading =false
}

const closeDialog = () => {
  state.dialog = false;
  rect = {}
};
const zip = (arr, ...arrs) => {
  return arr.map((val, i) => arrs.reduce((a, arr) => [...a, arr[i]], [val]));
};

const onAddProduct = (a) => {
  if (a) {

    emit("onProductItemAdded", a);
  }
  closeDialog();
};
</script>

<template>
  <section class="content">
    <video ref="camera" id="camera" @resize="updateCanvas" autoplay></video>
    <canvas ref="canvas" id="canvas" class="cam"></canvas>
    <canvas ref="overlay" id="overlay" class="cam"></canvas>
    <AddProductItem 
      :dialog="state.dialog" 
      :labels="props.labels"  
      :imageData="state.imageData"
      :selectedArea="state.selectedArea"
      @onAddProduct="onAddProduct"/>
          <v-btn
        class="ma-2"
        outlined
        color="indigo"
        @click="scan"
      >
      
      <v-progress-circular
        indeterminate
        color="white"
      ></v-progress-circular>
      scan
    </v-btn>

  </section>
</template>

<style scoped>
.content {
  margin-top: 10px;
  position: relative;
  flex: auto;
}
video {
  display: none;
  position: absolute;
  top: 0;
  left: 0;
}
canvas.cam {
  background: transparent;
  position: absolute;
  top: 0;
  left: 0;
}

</style>