# Storing Rules in JSON Format

Normally, rules in NRules are written using a DSL in C#. They are then converted into a canonical model (AST), which is then compiled to an executable model (Rete network). However, with the help of [NRules.Json](xref:NRules.Json) library the canonical rules model can also be serialized into JSON and deserialized from it. This could be useful in scenarios where rules need to be externalized from the application in a textual form.

## Serializing and Deserializing Rules
[NRules.Json](xref:NRules.Json) uses [JsonSerializer](xref:System.Text.Json.JsonSerializer) from `System.Text.Json` for serialization, so to make it work you need to set up an instance of [JsonSerializerOptions](xref:System.Text.Json.JsonSerializerOptions), such that it knows how to serialize/deserialize NRules types.

> [!WARNING]
> Serialization/deserialization of code and CLR types has an associated intrinsic security risk. Care must be taken to not deserialize rules from untrusted sources.

In the simplest form, serialization/deserialization code looks like this:
```c#
IRuleDefinition rule = BuildRule();

//Set up JsonSerializerOptions
var options = new JsonSerializerOptions
{
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
};
RuleSerializer.Setup(options);

//Serialize a rule into JSON
var json = JsonSerializer.Serialize(rule, options);

//Deserialize a rule from JSON
var ruleClone = JsonSerializer.Deserialize<IRuleDefinition>(json, options);
```

## Type Serialization
While the above code works, the default serialization of CLR type names is quite verbose, e.g. `System.Console, System.Console, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a`. You can use the [TypeResolver](xref:NRules.Json.TypeResolver) to configure aliases for known types, and pass it to the serializer setup method.

```c#
var typeResolver = new TypeResolver();
typeResolver.RegisterDefaultAliases();
typeResolver.RegisterAlias("Customer", typeof(Customer));
typeResolver.RegisterAlias("Order", typeof(Order));
typeResolver.RegisterAlias("Context", typeof(IContext));
typeResolver.RegisterAlias("Console", typeof(Console));

RuleSerializer.Setup(options, typeResolver);
```

You can also take full control of the type name serialization/deserialization by implementing a custom [ITypeResolver](xref:NRules.Json.ITypeResolver).

## Example Rule in JSON
Here is an example of a rule in JSON format, serialized with the above options. See samples for full details.
```json
{
  "name": "John Do Large Order Rule",
  "leftHandSide": {
    "elementType": "And",
    "childElements": [
      {
        "elementType": "Pattern",
        "name": "customer",
        "type": "Customer",
        "expressions": [
          {
            "name": "Condition",
            "expression": {
              "nodeType": "Lambda",
              "parameters": [
                {
                  "name": "customer",
                  "type": "Customer"
                }
              ],
              "body": {
                "nodeType": "Equal",
                "left": {
                  "nodeType": "MemberAccess",
                  "memberType": "Property",
                  "name": "Name",
                  "declaringType": "Customer",
                  "expression": {
                    "nodeType": "Parameter",
                    "name": "customer",
                    "type": "Customer"
                  }
                },
                "right": {
                  "nodeType": "Constant",
                  "type": "string",
                  "value": "John Do"
                }
              }
            }
          }
        ]
      },
      {
        "elementType": "Pattern",
        "name": "order",
        "type": "Order",
        "expressions": [
          {
            "name": "Condition",
            "expression": {
              "nodeType": "Lambda",
              "parameters": [
                {
                  "name": "order",
                  "type": "Order"
                },
                {
                  "name": "customer",
                  "type": "Customer"
                }
              ],
              "body": {
                "nodeType": "Equal",
                "left": {
                  "nodeType": "MemberAccess",
                  "memberType": "Property",
                  "name": "Customer",
                  "declaringType": "Order",
                  "expression": {
                    "nodeType": "Parameter",
                    "name": "order",
                    "type": "Order"
                  }
                },
                "right": {
                  "nodeType": "Parameter",
                  "name": "customer",
                  "type": "Customer"
                }
              }
            }
          },
          {
            "name": "Condition",
            "expression": {
              "nodeType": "Lambda",
              "parameters": [
                {
                  "name": "order",
                  "type": "Order"
                }
              ],
              "body": {
                "nodeType": "GreaterThan",
                "left": {
                  "nodeType": "MemberAccess",
                  "memberType": "Property",
                  "name": "Amount",
                  "declaringType": "Order",
                  "expression": {
                    "nodeType": "Parameter",
                    "name": "order",
                    "type": "Order"
                  }
                },
                "right": {
                  "nodeType": "Constant",
                  "type": "double",
                  "value": 100
                }
              }
            }
          }
        ]
      }
    ]
  },
  "rightHandSide": [
    {
      "expression": {
        "nodeType": "Lambda",
        "parameters": [
          {
            "name": "ctx",
            "type": "Context"
          }
        ],
        "body": {
          "nodeType": "Call",
          "methodName": "WriteLine",
          "declaringType": "Console",
          "arguments": [
            {
              "nodeType": "Constant",
              "type": "string",
              "value": "Found large order"
            }
          ]
        }
      }
    }
  ]
}
```
