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
interface SearchResult {
  clientId: string,
  startTime: Date,
  endTime: Date,
  durationInMs: number,
  products: ProductInfo[]
}
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
  clientId = "";

  mounted() {
    this.connectToNotificationHub();
  }

  private async connectToNotificationHub() {
    const response = await fetch(`http://localhost:8082/get-new-guid`);
    this.clientId = await response.text();
    const connection = new HubConnectionBuilder()
      .withUrl(`http://localhost:8082/notificationHub?client-id=${this.clientId}`)
      .build();

    connection.on("ReceivePrice", (data: SearchResult) => {
      fetch('http://localhost:8083/Add', {
        method: 'POST',
        cache: 'no-cache',
        headers: {
          'content-type': 'application/json;charset=UTF-8',
        },
        body: JSON.stringify({
          clientId: data.clientId,
          startTime: data.startTime,
          endTime: data.endTime,
          durationInMs: data.durationInMs
        })
      });

      this.searchResult = [];
      this.searchResult.push(...data.products);
      this.isLoading = false;
    });

    await connection.start();
    this.isSeachButtonDisabled = false;
  }

  search(): void {
    if (this.selectedProduct) {
      this.isLoading = true;
      fetch(`http://localhost:8081/search-product/${this.selectedProduct}/${this.clientId}`)
    }
  }
}
</script>
