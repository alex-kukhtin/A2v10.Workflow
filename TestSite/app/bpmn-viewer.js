
import BpmnViewer from "bpmn-js/lib/NavigatedViewer";

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

window.BpmnViewer = BpmnViewer;
