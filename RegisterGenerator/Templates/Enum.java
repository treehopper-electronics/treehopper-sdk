package io.treehopper.libraries.{{Package}};

{{#Enum}}
{{#IsPublic}}public{{/IsPublic}} enum {{Name}}
{
{{#ValuesList}}
    {{Key}} {{#Value}}({{Value}}){{^Last}},{{/Last}}{{#Last}};{{/Last}}
    {{/Value}}
{{/ValuesList}}

int val;

{{Name}}(int val) { this.val = val; }
public int getVal() { return val; }
}
{{/Enum}}