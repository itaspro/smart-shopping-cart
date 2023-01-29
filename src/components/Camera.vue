<script setup>
import { reactive, ref  , onMounted } from "vue";
const state = reactive ({model: null, isLoading : false})
const camera = ref(null);
onMounted(async () => {
  console.log(state)
  state.isLoading = true
  let m = new cvstfjs.ObjectDetectionModel();
  await m.loadModelAsync("model.json");
  state.model = m
  console.debug("model loaded.")
  await openCamera();
  state.isLoading = false
})

async function checkout() {
}

const openCamera = async () => {
    const constraints = (window.constraints = {
				audio: false,
				video: true
		});

		navigator.mediaDevices
			.getUserMedia(constraints)
			.then(stream => {
        state.isLoading = false;
        camera.value.srcObject = stream
			})
			.catch(e => {
        state.isLoading = false;
        console.error(e)
				alert("cannot open the camera!");
			});
}

</script>

<template>
  <button type="button" class="button" @click="checkout">
      <img src="https://img.icons8.com/material-outlined/50/000000/camera--v2.png">
  </button>
  <video ref="camera" :width="450" :height="337.5" autoplay></video>
</template>
