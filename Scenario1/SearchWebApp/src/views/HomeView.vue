<template>
 <loading v-model:active="isLoading" :is-full-page="true" />
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
        <button @click="search" class="btn btn-outline-primary" type="button" id="button-search" :disabled="isSeachButtonDisabled">Search</button>
      </div>
     </div>
  </div>
  <div class="row">
    <div class="col-12">
      <table class="table">
        <thead>
          <tr>
            <th scope="col">Store</th>
            <th scope="col">Product</th>
            <th scope="col">Price</th>
          </tr>
        </thead>
        <tbody v-for="item in searchResult" v-bind:key="item.store">
          <tr>
            <td>{{item.store}}</td>
            <td>{{item.name}}</td>
            <td>{{item.price}}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
 </div>
</template>

<script lang="ts">
import { Options, Vue } from 'vue-class-component';
import { HubConnectionBuilder } from '@microsoft/signalr';
import Loading from 'vue-loading-overlay';
import 'vue-loading-overlay/dist/css/index.css';
interface ProductInfo {
    store: string;
    name: string;
    price: number;
}

@Options({
  components: {
    Loading
  }
})
export default class HomeView extends Vue {
  selectedProduct = "";
  searchResult: ProductInfo[] = [];
  isLoading = false;
  isSeachButtonDisabled = true;
  requestId = "";

  mounted() {
    this.connectToNotificationHub();
  }

  private async connectToNotificationHub() {
    const response = await fetch(`http://localhost:8082/get-new-guid`);
    this.requestId = await response.text();
    const connection = new HubConnectionBuilder()
      .withUrl(`http://localhost:8082/notificationHub?request-id=${this.requestId}`)
      .build();

    connection.on("ReceivePrice", (data: ProductInfo[]) => {
      this.searchResult = [];
      this.searchResult.push(...data);
      this.isLoading = false;
    });

    await connection.start();
    this.isSeachButtonDisabled = false;
  }

  search(): void {
    if (this.selectedProduct) {
      this.isLoading = true;
      fetch(`http://localhost:8081/search-product/${this.selectedProduct}/${this.requestId}`)
    }
  }
}
</script>
