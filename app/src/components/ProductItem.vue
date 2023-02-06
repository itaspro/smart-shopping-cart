<script setup>
  import {  ref,reactive, onMounted, computed, watch  } from "vue";
  import VueNumberInput from '@chenfengyuan/vue-number-input';
  const pic = ref(null)
  const props = defineProps(['product'])
  const states = reactive({ count: 1 })
  const emit = defineEmits("onProductUpdated")
  onMounted(() =>{
    console.log(props.product.imageData)
    let ctx = pic.value.getContext('2d')
    ctx.putImageData(props.product.imageData,0,0)
  })

  const total = computed(() => {
    return states.count * parseFloat(props.product.price)
  })

  watch(() => states.count, () => {
    emit("onProductUpdated", {...props.product, count: states.count})
  })
</script>

<template>
  <v-container>
    <v-row>
      <v-col :cols="2">
        <canvas id="pic" ref="pic" class="product-image"></canvas>
      </v-col>
      <v-col :cols="6">
        <h4 class="product-name">{{product.label}}</h4>
        <p class="product-desc">{{product.sentence}}</p>
        <div class="price product-single-price">${{product.price}}</div>
      </v-col>
      <v-col :cols="3">
                  <vue-number-input v-model="states.count" inline center controls :min="1" :max="10" ></vue-number-input>
                  <button class="_btn entypo-trash product-remove">Remove</button>
                  <div class="price product-total-price">${{ total }}</div>
      </v-col>
    </v-row>
  </v-container>  
</template>

<style scoped>
.product-image {
  max-width: 200px;
  padding: 10px;
}
.product-info {
  padding: .5rem;
}
.product-name {
  font: 300 2em/1 "Lato", sans-serif;
  letter-spacing: -.025em;
  margin: 0 0 .125em;
}

.price {
  line-height: 1;
  text-align: right;
}
.product-single-price {
  margin-top: -1rem;
}

.product-modifiers {
  text-align: right;
  padding: 5px;
  border-left: 1px solid #bdc3c7;
}
.product-subtract,
.product-plus,
.product-qty {
  width: 33.330557%;
  background-color: transparent;
  color: #686868;
  text-align: center;
}
.product-qty {
  padding: .35em .75em;
}
.product-remove {
  font-size: .875em;
  margin-top: 1rem;
  background-color: #e74c3c;
  color: #ecf0f1;
  width: 100%;
  visibility: hidden;
}
.product-modifiers:hover .product-remove {
  visibility: visible;
}
.product-remove:before {
  margin-right: .5em;
}
.product-remove:hover {
  background-color: #c0392b;
}
.product-total-price {
  border-top: 1px solid #bdc3c7;
  color: #95a5a6;
  font-size: 1.25em;
  padding: .5rem;
} 

</style>