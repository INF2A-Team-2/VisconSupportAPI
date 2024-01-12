# Viscon Support API

[![Build](https://github.com/INF2A-Team-2/VisconSupportAPI/actions/workflows/dotnet.yml/badge.svg?branch=master&event=push)](https://github.com/INF2A-Team-2/VisconSupportAPI/actions/workflows/dotnet.yml)

## Usage
### Query filters
#### Sorting
##### `sort={key}.{asc/desc}`  
The key can be the C# Property `FooBar` or the JSON property `fooBar`.  
`asc` for ascending order and `desc` for descending order.  
Example: `sort=Id.asc` `sort=dateOfBirth.desc`

#### Limiting
##### `limit={n}`
Positive integer specifying how many items to include.  
If no limit is set the response includes all items.  
Example: `limit=1` `limit=100`
