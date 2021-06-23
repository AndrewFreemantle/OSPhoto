<template>
  <nav aria-label="breadcrumb">
    <ol class="breadcrumb">
      <li v-if="data.location.length === 0" class="breadcrumb-item active"><i class="fas fa-home"></i></li>

      <template v-if="data.location.length > 0">
        <li class="breadcrumb-item"><router-link to="/" class="link-dark text-decoration-none"><i class="fas fa-home"></i></router-link></li>

        <li v-for="(loc, index) in data.location" v-bind:key="loc" class="breadcrumb-item">
          <router-link :to="{ name: 'album', params: { album: loc.path }}" v-if="index + 1 !== data.location.length">{{loc.name}}</router-link>
          <span v-if="index + 1 === data.location.length" class="active">{{loc.name}}</span>
        </li>
      </template>
    </ol>
  </nav>
</template>

<script>
import { Store } from '../store';

export default {
  name: 'Location',
  data() {
    return {
      data: Store.state
    }
  },
  methods: {
    async updateStore() {
      // on page reload the router doesn't pick up the params/hash from the URL
      //  even though the docs say this should work as we're using Hash Mode:
      //  https://next.router.vuejs.org/guide/essentials/history-mode.html#hash-mode

      // a workaround is to look at the params first then at the location.hash
      let path = (this.$route.params && this.$route.params.album)
        ? this.$route.params.album
        : this.getPathFromUrl();

      Store.update(await this.fetchAlbum(path));
    },
    getPathFromUrl() {
      let hash = window.location.hash;
      if (hash == null) { return null; }
      if (hash.length <= 2) { return null; }

      // strip the '#/' and return the remainder..
      return hash.substring(2);
    },
    async fetchAlbum(path) {
      let url = `${process.env.VUE_APP_API_ROOT}/album` + ((path == null)
        ? ''
        : `/${path}`);

      const response = await fetch(url);
      return await response.json();
    },
  },
  async created() {
    await this.updateStore();

    // watch for changes to the route params
    this.$watch(_ => this.$route.params, async _ => await this.updateStore());
  },
}
</script>

<style scoped>
.breadcrumb {
  display: flex;
  flex-wrap: wrap;
  padding: 0 0;
  margin-bottom: 1rem;
  list-style: none;
}

.breadcrumb-item + .breadcrumb-item::before {
  float: left;
  padding-right: .5rem;
  color: #6c757d;
  content: "/";
}

.breadcrumb-item + .breadcrumb-item {
  padding-left: .5rem;
}

</style>
