# CSV Demo


## RestJsonProvider

Minimal Api to provide bank account demo data as json.


## JsonTransformer

Azure Timed Function that downloads a json file from an anonymous Web API and transforms it in a generic way to a csv, it saves the CSV on azure blob storage.


## CsvVisualizer

Forms Gui app that periodically checks azure blob storage for a csv and displays it.