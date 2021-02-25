﻿
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
	create: function (canvas) {
		if (typeof (canvas) === 'string')
			canvas = document.getElementById(canvas);
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
