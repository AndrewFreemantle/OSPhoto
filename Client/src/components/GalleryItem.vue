<template>

  <div v-if="item.isDirectory" @click="onDirectoryClick" class="gallery-item directory">
    <figure class="figure figure-dir">
      <i class="far fa-5x fa-folder"></i>
    </figure>

    <figcaption class="figure-caption">{{ item.name }}</figcaption>
  </div>

  <div v-if="item.isImage" @click="onImageClick" class="gallery-item image">
    <figure class="figure figure-img">
      <img :src="imagePath()" :alt="item.name" :title="item.name" />
    </figure>
  </div>

 <!-- TODO: show and hide files based on a toggle setting -->
<!--  <div v-if="!item.isDirectory && !item.isImage" class="gallery-item file">-->
<!--    <figure class="figure figure-file">-->
<!--      <i class="far fa-5x fa-file"></i>-->
<!--    </figure>-->

<!--    <figcaption class="figure-caption">{{ item.name }}</figcaption>-->
<!--  </div>-->

</template>

<script>
export default {
	name: 'GalleryItem',
  props: {
    item: {}
  },
	methods: {
		imagePath: function() {
			return `${process.env.VUE_APP_API_ROOT}/image/${this.item.path}`;
		},
    onDirectoryClick: function() {
      console.log('[dir.click]', this.item.path );
      this.$router.push({name: 'album', params: { album: this.item.path }});
    },
    onImageClick: function() {
      alert('image..');
    }
	}
}
</script>

<style>

.gallery-item {
  text-align: center;
  cursor: pointer;
  margin: 0 1rem 1rem 1rem;
}

.gallery-item:hover {
  background-color: rgba(155, 77, 202, 0.25);
  border-radius: 5px;
}

figure {
  margin: 1rem;

}

.figure-img {
  vertical-align: center;
}

.figure-img > img {
  max-width: 150px;
  max-height: 150px;
}
</style>