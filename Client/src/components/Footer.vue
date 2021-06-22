<template>
	<div id="footer">
		<div class="left float-left">
			<ul>
				<li><i class="far fa-images"></i> OSPhoto</li>
				<li><a href="https://github.com/AndrewFreemantle/OSPhoto" alt=""><i class="fab fa-github"></i> open source</a></li>
				<li><a href="https://github.com/AndrewFreemantle/OSPhoto/issues" alt=""><i class="far fa-comment-alt"></i> feedback</a></li>
				<li><a href="https://www.buymeacoffee.com/fatlemon"><i class="fas fa-mug-hot"></i> buy us a coffee!</a></li>
			</ul>
		</div>
		<div class="right float-right">
			v0.1.0
		</div>
	</div>
</template>

<script>

export default {
	name: 'Footer',
}

</script>

<style scoped>

#footer {
	position: absolute;
	bottom: 0;
	min-width: 99%;

	border-top: 1px solid rgba(155, 77, 202, 0.25);
	padding-top: .2rem;
	font-size: 1.4rem;
}

#footer ul {
  display: flex;
  flex-wrap: wrap;
  padding: 0 0;
  list-style: none;
	margin-bottom: 0;
}

li {
	margin-bottom: 0;
}

li + li::before {
  float: left;
  padding-right: 1rem;
  color: #6c757d;
  content: "|";
}

li + li {
  padding-left: 1rem;
}

</style>