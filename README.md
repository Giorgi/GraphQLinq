# GraphQLinq

LINQ to GraphQL - Strongly typed GraphQL queries with LINQ query syntax.

![Project Icon](Icon.png "GraphQLinq Project Icon")

## About The Project

GraphQLinq is a .NET tool for generating C# classes from a GraphQL endpoint and a C# library for writing strongly typed GraphQL queries with LINQ.

With GraphQLinq you will:

- Write strongly typed queries with LINQ.
- Have your queries checked by the compiler.
- Run queries and deserialize response into strongly typed classes in a single method call.

## Getting Started

### Installing Scaffolding Tool

Before you starting writing queries, you need to generate classes from GraphQL types. This is done by `GraphQLinq.Scaffolding`, a .NET tool that is part of the project.

To get the tool, open your favourite command shell and run

```sh
dotnet tool install --global GraphQLinq.Scaffolding
```

This will install the `GraphQLinq.Scaffolding` tool and make it available globally for all projects.

### Scaffolding Client Code

Next, navigate to the project where you want to add the classes and scaffold the client code. In this example I will use the [SpaceX GraphQL Api](https://api.spacex.land/graphql) so run the following command:

```sh
dotnet tool run graphql-scaffold https://api.spacex.land/graphql -o SpaceX -n SpaceX
```

The `o` option specifies the output directory for generated classes and `n` specifies the namespace of the classes.

![Scaffolding](Scaffolding.gif "Scaffolding GraphQL Client")

## Running GraphQL Queries with LINQ

The scaffolding tool generates classes for types available in the GraphQL type system and a `QueryContext` class that serves as an entry point for running the queries. GraphQLinq supports running different types of queries.

### Select all Primitive Properties of a Type

To select all properties of a type simply run a query like this:

```cs
var spaceXContext = new QueryContext();

var company = spaceXContext.Company().ToItem();

RenderCompanyDetails(company);
```

This will select all primitive and string properties of `Company` but it won't select nested properties or collection type properties. Here is the output of the code snippet:

```sh
┌───────────┬──────────────────────────────────────────────────────────────────────────────────────────────────────────┐
│ Property  │ Value                                                                                                    │
├───────────┼──────────────────────────────────────────────────────────────────────────────────────────────────────────┤
│ Name      │ SpaceX                                                                                                   │
│ Ceo       │ Elon Musk                                                                                                │
│ Summary   │ SpaceX designs, manufactures and launches advanced rockets and spacecraft. The company was founded in    │
│           │ 2002 to revolutionize space technology, with the ultimate goal of enabling people to live on other       │
│           │ planets.                                                                                                 │
│ Founded   │ 2002                                                                                                     │
│ Founder   │ Elon Musk                                                                                                │
│ Employees │ 7000                                                                                                     │
└───────────┴──────────────────────────────────────────────────────────────────────────────────────────────────────────┘
```

### Select Specific Properties

If you want to select specific properties, including a navigation property, you can specify it with the `Select` method. You either map the projection to an existing type or an anonymous object (`Headquarters` is a nested property):

```cs
var companySummaryAnonymous = spaceXContext.Company().Select(c => new { c.Ceo, c.Name, c.Headquarters }).ToItem();

//Use data class to select specific properties
var companySummary = spaceXContext.Company().Select(c => new CompanySummary
{
    Ceo = c.Ceo,
    Name = c.Name,
    Headquarters = c.Headquarters
}).ToItem();

RenderCompanySummary(companySummary);
```

This will result in the following output:

```sh
┌──────────────┬─────────────────┐
│ Property     │ Value           │
├──────────────┼─────────────────┤
│ Name         │ SpaceX          │
│ Ceo          │ Elon Musk       │
│ Headquarters │ ┌─────────────┐ │
│              │ │ California  │ │
│              │ │ Hawthorne   │ │
│              │ │ Rocket Road │ │
│              │ └─────────────┘ │
└──────────────┴─────────────────┘
```