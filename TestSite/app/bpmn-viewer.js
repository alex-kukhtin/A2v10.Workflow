
import BpmnViewer from "bpmn-js/lib/NavigatedViewer";

import modeling from 'bpmn-js/lib/features/modeling';


/*see: https://github.com/bpmn-io/bpmn-js/tree/develop/lib*/

/*
let canvas = document.getElementById('canvas');

let bpmnViewer = new BpmnViewer({
	container: canvas,
	keyboard: {
		bindTo: window
	}
});
*/

window.BpmnViewer = {
	create: function (canvasId) {
		let canvas = document.getElementById(canvasId);
		return new BpmnViewer({
			container: canvas,
			keyboard: {
				bindTo: window
			},
			additionalModules: [
				modeling
			]
		});
	}
};
