# Viscon Support API

[![Build](https://github.com/INF2A-Team-2/VisconSupportAPI/actions/workflows/dotnet.yml/badge.svg?branch=master&event=push)](https://github.com/INF2A-Team-2/VisconSupportAPI/actions/workflows/dotnet.yml)

## Usage
### Query filters
#### Sorting
##### `sort={key}.{asc/desc}`  
The key can be the C# Property `FooBar` or the JSON property `fooBar`.  
`asc` for ascending order and `desc` for descending order.

#### Limiting
##### `limit={n}`
Positive integer specifying how many items to include.
