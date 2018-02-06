package io.treehopper.libraries.{{Package}};

{{#Enum}}
{{#IsPublic}}public{{/IsPublic}} enum {{PluralizedName}}
{
{{#ValuesList}}
    {{Key}} {{#Value}}({{Value}}){{^Last}},{{/Last}}{{#Last}};{{/Last}}
    {{/Value}}
{{/ValuesList}}

int val;

{{PluralizedName}}(int val) { this.val = val; }
public int getVal() { return val; }
}
{{/Enum}}