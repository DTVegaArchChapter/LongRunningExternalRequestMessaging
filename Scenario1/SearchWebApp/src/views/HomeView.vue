<template>
 <div class="container">
  <div class="row">
    <div class="m-2">
            <div class="input-group">
              <select v-model="selectedProduct" class="form-select" aria-label="Select Product">
                <option value="">Select Product..</option>
                <option value="1">Cell Phone</option>
                <option value="2">Laptop</option>
                <option value="3">Desktop</option>
              </select>
              <button @click="search" class="btn btn-outline-primary" type="button" id="button-search">Search</button>
            </div>
     </div>
  </div>
 </div>
</template>

<script lang="ts">
import { Options, Vue } from 'vue-class-component';

@Options({})
export default class HomeView extends Vue {
  selectedProduct = ""

  search(): void {
    if (this.selectedProduct) {
      fetch(`http://localhost:54484/search-product/${this.selectedProduct}`)
    }
  }
}
</script>
