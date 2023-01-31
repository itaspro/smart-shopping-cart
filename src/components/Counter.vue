<script setup>

import { reactive, ref , onMounted, nextTick} from "vue";
const state = reactive ({
  model: null, 
  isLoading : false,
  products: [],
  labels: []
})
const camera = ref(null)
const canvas = ref(null)
const model = ref(null)

onMounted(async () => {
  state.isLoading = true
  await loadLabels()
  await openCamera()
  state.isLoading = false
})

let loadLabels = async () => {
  let resp = await fetch('v4/labels.txt')
  state.labels = (await resp.text()).split("\n")
};

async function checkout() {
  let model = new cvstfjs.ObjectDetectionModel();
  await model.loadModelAsync("/v4/model.json");
  let tensor = tf.browser.fromPixels(camera.value, 3)
		.resizeNearestNeighbor([416, 416]) // change the image size
		.expandDims()
		.toFloat()
		.reverse(-1); // RGB -> BGR
  let data = await model.executeAsync(tensor);
  let products = zip(...data)
    .filter(d => d[1] > 0.5)
    .map(p => [...p, state.labels[p[2]]])
  console.log(products);
}

const draw = () => {
  const context = canvas.value.getContext('2d');
  context.drawImage(camera.value,0,0);
  window.requestAnimationFrame(draw);
}

const zip = (arr, ...arrs) => {
  return arr.map((val, i) => arrs.reduce((a, arr) => [...a, arr[i]], [val]));
}

const openCamera = async () => {
    const openMediaDevices = async (constraints) => {
      return await navigator.mediaDevices.getUserMedia(constraints);
    };

    try {
        const stream = await openMediaDevices({'video':true,'audio':false});
        camera.value.srcObject = stream
    } catch(error) {
        console.error('Error accessing camera.', error);
    }
    window.requestAnimationFrame(draw);
}
</script>

<template>
  <div>
    <button type="button" class="button" @click="checkout">
        <img src="https://img.icons8.com/material-outlined/50/000000/camera--v2.png">
    </button>
    <video ref="camera" hidden="true" id="camera" :width="450" :height="337.5" autoplay></video>
    <canvas ref="canvas"  id="canvas" :width="450" :height="337.5"></canvas>

  </div>
  <p id="products">products</p>
</template>
