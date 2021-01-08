export default {
	"name": "Workflow",
	"prefix": "workflow",
	"uri": "http://workflow",
	"xml": {
		"tagAlias": "lowerCase"
	},
	"associations": [],
	"types": [
		{
			"name": "BewitchedStartEvent",
			"extends": [
				"bpmn:StartEvent"
			],
			"properties": [
				{
					"name": "spell",
					"isAttr": true,
					"type": "String"
				}
			]
		}
	]
};