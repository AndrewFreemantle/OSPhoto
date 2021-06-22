import { reactive } from 'vue';

// Simple state management store
// https://v3.vuejs.org/guide/state-management.html#simple-state-management-from-scratch
export const Store = {
	state: reactive({
		location: [],
		contents: [],
	}),

	update(newState) {
		if (newState && newState.location) { this.state.location = newState.location; }
		if (newState && newState.contents) { this.state.contents = newState.contents; }
	},

};