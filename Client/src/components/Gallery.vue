<template>
  <div class="gallery">
	  <GalleryItem v-for="item in data.contents" :item="item" v-bind:key="item"></GalleryItem>
  </div>
</template>

<script>
import { Store } from '../store';
import GalleryItem from './GalleryItem';

export default {
  name: 'Gallery',
	components: {
		GalleryItem
	},
  data() {
    return {
      data: Store.state
    }
  },
}
</script>


<style scoped>

.gallery {
  display: flex;
  flex-flow: row wrap;
  align-items: center;
  flex-grow: 1;
}

</style>