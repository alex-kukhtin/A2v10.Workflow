import inherits from 'inherits';

import PropertiesActivator from 'bpmn-js-properties-panel/lib/PropertiesActivator';

// Require all properties you need from existing providers.
// In this case all available bpmn relevant properties without camunda extensions.
import processProps from 'bpmn-js-properties-panel/lib/provider/bpmn/parts/ProcessProps';
import eventProps from 'bpmn-js-properties-panel/lib/provider/bpmn/parts/EventProps';
import linkProps from 'bpmn-js-properties-panel/lib/provider/bpmn/parts/LinkProps';
import documentationProps from 'bpmn-js-properties-panel/lib/provider/bpmn/parts/DocumentationProps';
import idProps from 'bpmn-js-properties-panel/lib/provider/bpmn/parts/IdProps';
import nameProps from 'bpmn-js-properties-panel/lib/provider/bpmn/parts/NameProps';


// Require your custom property entries.
import spellProps from './parts/SpellProps';


// The general tab contains all bpmn relevant properties.
// The properties are organized in groups.
function createGeneralTabGroups(element, bpmnFactory, canvas, elementRegistry, translate) {

	var generalGroup = {
		id: 'general',
		label: 'General',
		entries: []
	};
	idProps(generalGroup, element, translate);
	nameProps(generalGroup, element, bpmnFactory, canvas, translate);
	processProps(generalGroup, element, translate);

	var detailsGroup = {
		id: 'details',
		label: 'Details',
		entries: []
	};
	linkProps(detailsGroup, element, translate);
	eventProps(detailsGroup, element, bpmnFactory, elementRegistry, translate);

	var documentationGroup = {
		id: 'documentation',
		label: 'Documentation',
		entries: []
	};

	documentationProps(documentationGroup, element, bpmnFactory, translate);

	console.dir('createGeneralTabGroups')
	return [
		generalGroup,
		detailsGroup,
		documentationGroup
	];
}

// Create the custom magic tab
function createWorkflowTabGroups(element, translate) {

	// Create a group called "Black Magic".
	var blackMagicGroup = {
		id: 'wf',
		label: 'Workflow',
		entries: []
	};

	// Add the spell props to the black magic group.
	spellProps(blackMagicGroup, element, translate);

	return [
		blackMagicGroup
	];
}

export default function WorkflowPropertiesProvider(
	eventBus, bpmnFactory, canvas,
	elementRegistry, translate) {

	PropertiesActivator.call(this, eventBus);

	this.getTabs = function (element) {

		var generalTab = {
			id: 'general',
			label: 'General',
			groups: createGeneralTabGroups(element, bpmnFactory, canvas, elementRegistry, translate)
		};

		// The "workflow" tab
		var workflowTab = {
			id: 'wf',
			label: 'Workflows',
			groups: createWorkflowTabGroups(element, translate)
		};

		// Show general + "workflow" tab
		return [
			generalTab,
			workflowTab
		];
	};
}

inherits(WorkflowPropertiesProvider, PropertiesActivator);