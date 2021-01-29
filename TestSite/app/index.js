
import BpmnModeler from 'bpmn-js/lib/Modeler';

import propertiesPanelModule from 'bpmn-js-properties-panel';
import propertiesProviderModule from './provider/workflow';

import workflowModdleDescriptor from './descriptors/workflow';

let canvas = document.getElementById('canvas');

let bpmnModeler = new BpmnModeler({
	container: canvas,
	keyboard: {
		bindTo: window
	},
	propertiesPanel: {
		parent: '#js-properties-panel'
	},
	additionalModules: [
		propertiesPanelModule,
		propertiesProviderModule
	],
	moddleExtensions: {
		workflow: workflowModdleDescriptor
	}
});

window.Modeler = bpmnModeler;

window.addEventListener("beforeunload", function (ev) {
	let cs = bpmnModeler.get("commandStack");
	if (cs.canUndo()) {
		ev.preventDefault();
		event.returnValue = ''; // chrome
	}
});

bpmnModeler.on('commandStack.changed', function () {
	console.dir('commandStack.changed');
});