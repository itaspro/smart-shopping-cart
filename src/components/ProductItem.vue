<script setup>
  import {  ref, onMounted,  } from "vue";
  const pic = ref(null)
  const props = defineProps(['product'])
  onMounted(() =>{
    console.log(props.product.imageData)
    let ctx = pic.value.getContext('2d')
    ctx.putImageData(props.product.imageData,0,0)
  })
</script>

<template>
  <div class="_row">

          <div class="_column product-image">
            <!-- <img class="product-image--img" src="product.imageData" alt="Item image" /> -->
            <canvas class="product-image" ref="pic" id="canvas" ></canvas>
          </div>
          <div class="_column product-info">
            <h4 class="product-name">{{product.label}}</h4>
            <p class="product-desc">{{product.sentence}}</p>
            <div class="price product-single-price">${{product.price}}</div>
          </div>
          <div class="_column product-modifiers" data-product-price="{{product.price}}">
            <button class="_btn entypo-trash product-remove">Remove</button>
            <div class="price product-total-price">$0.00</div>
          </div>
  </div>
</template>

<style scoped>
.product-info {
  width: 70.832119%;
  padding: .5rem;
}
.product-name {
  font: 300 2.4em/1 "Lato", sans-serif;
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

/**
 * @section: product-modifiers;
 * @extends: _column;
 */
.product-modifiers {
  width: 12.496358%;
  text-align: right;
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
  margin-top: 3.35rem;
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
.shopping-cart--list-item:hover .product-total-price {
  background-color: #3498db;
  color: #ecf0f1;
}

.product-image {
  width: 100px;
}
</style>