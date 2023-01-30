<script setup>

import { reactive, ref , onMounted, nextTick} from "vue";
const state = reactive ({model: null, isLoading : false})
const camera = ref(null);
const canvas = ref(null);
const model = ref(null)

onMounted(async () => {
  state.isLoading = true
  // model.value = new cvstfjs.ObjectDetectionModel();
  // await model.value.loadModelAsync("/v4/model.json");
  await openCamera();
  state.isLoading = false
})

async function checkout() {
  let model = new cvstfjs.ObjectDetectionModel();
  await model.loadModelAsync("/v4/model.json");
  let tensor = tf.browser.fromPixels(camera.value, 3)
		.resizeNearestNeighbor([416, 416]) // change the image size
		.expandDims()
		.toFloat()
		.reverse(-1); // RGB -> BGR
  let result = await model.executeAsync(tensor);
  console.log(result)
  if (result[0].length > 0) {
    for (let n = 0; n < result[0].length; n++) {
      // Check scores
      if (result[1][n] > 0.5) {
        const p = document.getElementById("products");

        p.innerText = "label: " + parseFloat(result[2][n]+1) + " " +  Math.round(parseFloat(result[1][n]) * 100) + "%";
      }
    }
  }
}

const draw = () => {
  const context = canvas.value.getContext('2d');
  context.drawImage(camera.value,0,0);
  window.requestAnimationFrame(draw);
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
		// navigator.mediaDevices
		// 	.getUserMedia(constraints)
		// 	.then(stream => {
    //     state.isLoading = false;
    //     camera.value.srcObject = stream
		// 	})
		// 	.catch(e => {
    //     state.isLoading = false;
    //     console.error(e)
		// 		alert("cannot open the camera!");
		// 	});
    window.requestAnimationFrame(draw);
}

</script>

<template>
  <button type="button" class="button" @click="checkout">
      <img src="https://img.icons8.com/material-outlined/50/000000/camera--v2.png">
  </button>
  <video ref="camera" hidden="true" id="camera" :width="450" :height="337.5" autoplay></video>
  <canvas ref="canvas"  id="canvas" :width="450" :height="337.5"></canvas>
  <p id="products">products</p>
</template>
