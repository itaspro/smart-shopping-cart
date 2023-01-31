<script setup>
  const props = defineProps(['product'])
</script>

<template>
  <p>{{product.label}}</p>
          <div class="_column product-image">
          <!-- <img class="product-image--img" src="{{=img}}" alt="Item image" /> -->
          </div>
          <div class="_column product-info">
            <h4 class="product-name">{{product.label}}</h4>
            <p class="product-desc">{{product.sentence}}</p>
            <div class="price product-single-price">${{product.price}}</div>
          </div>
          <div class="_column product-modifiers" data-product-price="{{product.price}}">
            <div class="_grid">
              <button class="_btn _column product-subtract">&minus;</button>
              <div class="_column product-qty">0</div>
              <button class="_btn _column product-plus">&plus;</button>
            </div>
            <button class="_btn entypo-trash product-remove">Remove</button>
            <div class="price product-total-price">$0.00</div>
          </div>
</template>
