<template>
  <Location></Location>
  <router-view></router-view>
  <Footer></Footer>
</template>

<script>
import Location from './components/Location';
import Footer from './components/Footer.vue';

export default {
  name: 'App',
  components: {
    Location,
    Footer
  },
}
</script>

<style>

</style>
