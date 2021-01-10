
import entryFactory from 'bpmn-js-properties-panel/lib/factory/EntryFactory';

import elementHelper from 'bpmn-js-properties-panel/lib/helper/ElementHelper';
import cmdHelper from 'bpmn-js-properties-panel/lib/helper/CmdHelper';

import { is, getBusinessObject } from 'bpmn-js/lib/util/ModelUtil';

/*
var extensionElementsEntry = require('./ExtensionElements'),
	extensionElementsHelper = require('../../../../helper/ExtensionElementsHelper'),
	cmdHelper = require('../../../../helper/CmdHelper'),
	elementHelper = require('../../../../helper/ElementHelper'),
	ImplementationTypeHelper = require('../../../../helper/ImplementationTypeHelper');
*/


function generateValueId() {
	return utils.nextId('Value_');
}

const variablesHtml = `
<div class="bpp-row bpp-element-list">
	<label for=wf-variables-list>Activity variables</label>
	<div class=bpp-field-wrapper>
		<select size="5" class="bpp-variables-list" name="wf-variables-list"
			data-list-entry-container data-on-change=selectElement>
		</select>
		<button class="action-button add" data-action="createElement"><span>+</span></button>
		<button class="action-button clear" 
				data-action="removeElement" data-disable="disableRemove">
			<span>x</span>
		</button>
	</div>
</div>
`;

export default function addVariables(group, element, translate) {

	if (!is(element, 'bpmn:Process') && !is(element, 'bpmn:SubProcess'))
		return;
	group.entries.push({
		id: 'Variables',
		html: variablesHtml,
		get(elem, node) {
			console.dir('get from addVariables');
			console.dir({ elem, node });
			let bo = getBusinessObject(elem);
			console.dir({ elem, node, bo });
			var ee = bo.get("extensionElements");
			if (bo.extensionElements && bo.extensionElements.values) {
				// $Variables
				return bo.extensionElements.values;
			}
			return [];
		},
		createElement(elem, node) {
			alert('create element');
		},
		removeElement(elem, node) {
			alert('remove element');
		},
		disableRemove(elem, entryNode, node, scope) {
			console.dir('disable remove');
		},
		selectElement(elem, node, event, scope) {
			console.dir('select element');
			console.dir({ elem, node, event, scope });
		},
		createListEntryTemplate(value, index, selectbox) {
			console.dir('create entry template');
			console.dir({ value, index, selectbox });
			return `<option value="">OPTION FROM CREATE LIST ENTRY TEMPLATE</option>`;
		}
	});
}
