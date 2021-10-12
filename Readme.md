# Duplicate company name filtering
This project aims to use fuzzy matching to eliminate duplicate company names in a list, accounting for variations and typos.

## Approach
Input data is sanitised, trimming excess whitespace, converting accented characters to their corresponding base character counterparts, converting ampersands to 'and' and replacing numeric characters with words (e.g. 456 -> four hundred and fifty-six). Business terms like Co., Ltd., etc. are standardised, either by converting them to long-form (e.g. Company, Limited) or removing them entirely (toggleable behaviour). All text is converted to lower case.

Matching between entries is done using FuzzySharp, a C# implementation of the FuzzyWuzzy library. The Weighted Ratio function is used to generate a similarity score. This is slower than many of the simpler comparison functions included in the library, but performs better (the project currently takes ~50 seconds to execute on the test data set; simpler fuzzy matching functions tested reduced this to <30).

## Data
Input data is read in from test_data/org_names.json. The reduced list is saved in the same directory.

## Parameters
Takes three parameters, in order.
|                |                         ||
|----------------|-------------------------------|-----------------------------|
|Similarity threshold|int, 0-100 range; default 92            |Determines how similar names must be to be considered duplicates; 0 is most permissive, 100 least           |
|Strip business terms|bool; default true            |Determines whether business terms like co. and ltd. are standardised to long-form or removed entirely            |
|Output grouped duplicates list|bool; default true|Determines whether an additional file will be saved, containing a list of duplicate entries grouped by how they were matched|
