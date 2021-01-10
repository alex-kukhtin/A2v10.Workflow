
import entryFactory from 'bpmn-js-properties-panel/lib/factory/EntryFactory';

import elementHelper from 'bpmn-js-properties-panel/lib/helper/ElementHelper';
import cmdHelper from 'bpmn-js-properties-panel/lib/helper/CmdHelper';

import { is, getBusinessObject } from 'bpmn-js/lib/util/ModelUtil';
import extensionElementsImpl from './impl/extensionElements';

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

export default function addVariables(group, element, bpmnFactory, translate) {

	if (!is(element, 'bpmn:Process') && !is(element, 'bpmn:SubProcess'))
		return;
	group.entries.push({
		id: 'Variables',
		html: variablesHtml,
		get(elem, node) {
			console.dir('variables get-elements');
			let ee = extensionElementsImpl.getExtensionElement(elem, 'wf:Variables');
			return ee ? ee.values : [];
		},
		set(elem, values, node) {
			let action = this.__action;
			delete this.__action;
			if (!action) return;
			let vars = extensionElementsImpl.getExtensionElement(elem, 'wf:Variables');
			return cmdHelper.addElementsTolist(elem, vars, 'values', [action.value]);
		},
		createElement(elem, node) {
			let selbox = node.querySelector('select[name=wf-variables-list]');

			var bo = getBusinessObject(elem);
			let vars = extensionElementsImpl.getExtensionElement(elem, 'wf:Variables');
			var variable = elementHelper.createElement('wf:Variable', { Name: '', Type: 'String', Dir: 'Local' }, vars, bpmnFactory);
			var optTemplate = this.createListEntryTemplate(variable);
			let tmpsel = document.createElement('select');
			tmpsel.innerHTML = optTemplate;
			selbox.appendChild(tmpsel.firstChild);
			selbox.lastChild.selected = 'selected';
			this.__action = {
				id: 'add-variable',
				value: variable
			};
			return true;
			/*
			var commands = [];
			var bo = getBusinessObject(elem);
			let vars = extensionElementsImpl.getExtensionElement(elem, 'wf:Variables');
			var variable = elementHelper.createElement('wf:Variable', { Name: '', Type: 'String', Dir: 'Local' }, vars, bpmnFactory);
			commands.push(cmdHelper.addElementsTolist(bo, vars, 'values', [variable]));
			vars.values.push(variable);
			return commands;
			*/
		},
		removeElement(elem, node) {
			alert('remove element');
		},
		updateElement(elem, values, node, idx) {
			console.dir('update element');
			console.dir({ elem, values, node, idx});
		},
		disableRemove(elem, entryNode, node, scope) {
			let sel = extensionElementsImpl.getSelectedVariable(node);
			return !sel || sel.idx === -1;
		},
		selectElement(elem, node, event, scope) {
			console.dir('select element');
			console.dir({ elem, node, event, scope });
		},
		setControlValue(elem, node, option, property, value, idx) {
			let ee = extensionElementsImpl.getExtensionElement(elem, 'wf:Variables');
			if (!ee || idx >= ee.values.length) return;
			let varb = ee.values[idx];
			let tmlText = this.createTemplateText(varb);
			node.text = tmlText;
		},
		createTemplateText(value) {
			return `${value.Name}: ${value.Type}  [${value.Dir}]`;
		},
		createListEntryTemplate(value) {
			/*'data-value' is required
			 *'data-name' is parameter 'property' for 'setControlValue'.
			 */
			return `<option value="" data-value data-name="variableText">${this.createTemplateText(value)}</option>`;
		}
	});
}
