<script setup>
  import {reactive, ref, onMounted, watch} from 'vue'
  const props = defineProps(["dialog","imageData","labels","selectedArea"]);
  const selectedCanv = ref(null);
  const state = reactive({
    showDiaglog: false,
    selectedItem: {
      label: "",
      sku: null,
      saveForTrainging: true,
      argument: false,
    },
  });
  const emit = defineEmits("onAddProduct")
  watch(() => props.dialog, () => {
    state.showDiaglog = props.dialog
    
    if (props.imageData && props.dialog) {
      selectedCanv.value.height = props.selectedArea.height
      selectedCanv.value.width = props.selectedArea.width
      let ctx = selectedCanv.value.getContext("2d")
      ctx.putImageData(props.imageData, 0, 0);
    }
  })
  const productItemSelected = (f) => {
    if (f) {
      let sku = props.labels.findIndex((p) => p == state.selectedItem.label);
      let image = selectedCanv.value.toDataURL();
      emit("onAddProduct", {...state.selectedItem, imageData: props.imageData, sku, image,width: props.selectedArea.width, height: props.selectedArea.height })
    } else {
      emit("onAddProduct", null)
    }
  }
</script>
<template>  
  <v-dialog v-model="state.showDiaglog" eager>
    <v-card>
      <v-toolbar dark>
        <v-toolbar-title>Add Product Item</v-toolbar-title>
      </v-toolbar>

      <v-card-text>
        <v-container>
          <v-row>
            <v-col cols="4">
              <canvas
                    ref="selectedCanv"
                    id="selectedCanv"
                    class="selectedCanv"
                    eager
              ></canvas>
            </v-col>
              <v-col cols="8">
                <v-select
                    :items="props.labels"
                    label="Product Name*"
                    v-model="state.selectedItem.label"
                    required
                ></v-select>
                  <v-list lines="two" subheader>
                    <v-list-item
                      title="Enable Training"
                      subtitle="Image and Label will be saved and uploaded for model training"
                    >
                      <template v-slot:prepend>
                        <v-checkbox
                            v-model="state.selectedItem.saveForTrainging"
                        ></v-checkbox>
                      </template>
                    </v-list-item>
                    <v-list-item
                      title="Argument Image"
                      subtitle="Generate training images by rotating and translation"
                    >
                      <template v-slot:prepend>
                        <v-checkbox
                          v-model="state.selectedItem.argument"
                        ></v-checkbox>
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
              @click="productItemSelected(false)"
            >
              Close
            </v-btn>
            <v-btn
              color="blue-darken-1"
              variant="text"
              @click="productItemSelected(true)"
            >
              Save
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
</template>