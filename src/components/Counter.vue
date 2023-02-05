<script setup>
import * as cvstfjs from '@microsoft/customvision-tfjs';
import * as tf from '@tensorflow/tfjs';
import { reactive, ref, onMounted, nextTick } from "vue";
import ProductItem from "./ProductItem.vue";
  const props = defineProps(['threshold','labels'])
  const emit = defineEmits(['onDetected','onProductItemAdded'])
  const state = reactive({
    isLoading: false,
    dialog: false,
    selectedItem: {
      imageData: null,
      label: "",
      sku: null,
      saveForTrainging: false,
      arugmented: false
    }
  });

  const camera = ref(null)
  const canvas = ref(null)
  const overlay = ref(null)
  const selectedCanv = ref(null)

  let ctx = null
  let rect = {}
  let drag = false

  let model = null

  onMounted(async () => {
    console.log("xxx",props.labels)
    state.isLoading = true;
    model = await loadModel();
    await openCamera();
    state.isLoading = false;

    overlay.value.addEventListener('mousedown', mouseDown, false);
    overlay.value.addEventListener('mouseup', mouseUp, false);
    overlay.value.addEventListener('mousemove', mouseMove, false);
    ctx = overlay.value.getContext('2d')
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
        context.drawImage(camera.value, 0, 0, camera.value.videoWidth,camera.value.videoHeight);
      }
      window.requestAnimationFrame(draw);
    };

    window.requestAnimationFrame(draw);
  };

function mouseDown(e) {
  rect.startX = e.offsetX;
  rect.startY = e.offsetY ;
  drag = true;
}

function mouseUp() {
  drag = false;
  if (rect.w < 10) {
    return
  }
  state.dialog = true
  let ctx = canvas.value.getContext('2d')
  // console.log(rect)
  let imageData = ctx.getImageData(
  Math.round(rect.startX), 
  Math.round(rect.startY),
  Math.round(rect.w),
  Math.round(rect.h))

  state.selectedItem = {
    imageData: imageData,
    label: props.labels[0],
    saveForTrainging: true,
    arugmented: true,
    sku: 0
  }
  
  selectedCanv.value.width = rect.w
  selectedCanv.value.height = rect.h
  selectedCanv.value.getContext('2d').putImageData(imageData,0,0)
  overlay.value.getContext('2d').reset();
  rect.w =0
  rect.h = 0

}
function mouseMove(e) {
  if (drag) {
    rect.w = e.offsetX - rect.startX;
    rect.h = e.offsetY - rect.startY ;
    if (rect.w > 10 && rect.h > 10) {
      ctx.clearRect(0,0,overlay.value.width,overlay.value.height);
      ctx.setLineDash([6]);
      ctx.strokeRect(rect.startX, rect.startY, rect.w, rect.h);

    }
  }
}
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

const closeDialog = () => {
  state.dialog= false;
  rect = {}
}
const zip = (arr, ...arrs) => {
  return arr.map((val, i) => arrs.reduce((a, arr) => [...a, arr[i]], [val]));
};

const productItemSelected= () => {
  let sku = props.labels.findIndex(p => p == state.selectedItem.label)
  let image = selectedCanv.value.toDataURL()
  emit("onProductItemAdded", {...state.selectedItem, image, sku})
  closeDialog()
}
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
    <canvas ref="canvas" id="canvas" class="cam"></canvas>
    <canvas ref="overlay" id="overlay" class="cam"></canvas>
    <v-row justify="center">
      <v-dialog
          v-model="state.dialog"
          :scrim="false"
          eager
      >
      <v-card>
          <v-toolbar
            dark
          >
            <v-toolbar-title>Add Product Item</v-toolbar-title>
          </v-toolbar>

          <v-card-title>
          </v-card-title>
          <v-card-text>
            <v-container>
              <v-row>
                <v-col cols="4">
                  <canvas ref="selectedCanv" id="selectedCanv" class="selectedCanv" eager></canvas>
                </v-col>
                <v-col cols="8" >
                    <v-select
                      :items="props.labels"
                      label="Product Name*"
                      v-model="state.selectedItem.label"
                      required
                    ></v-select>
          <v-list
            lines="two"
            subheader
          >
            <v-list-item title="Enable Training" subtitle="Image and Label will be saved and uploaded for model training">
              <template v-slot:prepend>
                <v-checkbox v-model="state.selectedItem.saveForTrainging"></v-checkbox>
              </template>
            </v-list-item>
            <v-list-item title="Argument Image" subtitle="Generate training images by rotating and translation">
              <template v-slot:prepend>
                <v-checkbox v-model="state.selectedItem.arugmented"></v-checkbox>
              </template>
            </v-list-item>
            </v-list> 
                </v-col>
                </v-row>
              </v-container>
            </v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn
                color="blue-darken-1"
                variant="text"
                @click="state.dialog = false"
              >
                Close
              </v-btn>
              <v-btn
                color="blue-darken-1"
                variant="text"
                @click="productItemSelected"
              >
                Save
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>

    </v-row>
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
  canvas.cam {
    background:transparent;
    position: absolute;
    top: 0;
    left: 0;
  }
  
  button {
    /* position: fixed; */
    float: right;
    z-index: 999;
  }

  .v-card {
    max-width: 80%;
    min-width: 640px;
    margin: auto;
  }

  canvas.selectedCanv {
    max-width: 180px;
  }
</style>