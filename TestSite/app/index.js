﻿
import BpmnModeler from 'bpmn-js/lib/Modeler';

import propertiesPanelModule from 'bpmn-js-properties-panel';
import propertiesProviderModule from './provider/workflow';

import workflowModdleDescriptor from './descriptors/workflow';
//var workflowModdleDescriptor = require('./descriptors/workflow');

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

console.dir('custom properties6');

console.dir(bpmnModeler);
bpmnModeler.createDiagram();
