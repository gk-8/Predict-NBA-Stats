NBA Stats prediction
=================
Machine Learning experiment presented at [Microsoft Tech Summit 2016](https://www.microsoft.com/es-es/microsoftsummit/tech-summit.aspx). Using [Microsoft Azure Machine Learning Studio](http://studio.azureml.net) we'll clean the historic data, and use it to predict next season stats for the NBA players.

### Training Experiment
We'll take advantage of Azure Machine Learning Studio capabilities to transform, clean, structure and analyze our data in order to get the best results. 

![Training Schema (https://historicnbastats.blob.core.windows.net/images/training_schema.PNG)](https://historicnbastats.blob.core.windows.net/images/training_schema.PNG)

![Training Experiment 1(https://historicnbastats.blob.core.windows.net/images/training_exp1.PNG)](https://historicnbastats.blob.core.windows.net/images/training_exp1.PNG)
![Training Experiment 2(https://historicnbastats.blob.core.windows.net/images/training_exp2.PNG)](https://historicnbastats.blob.core.windows.net/images/training_exp2.PNG)

### Predictive Experiment
And then we can predict 2017 stats and use them from everywhere we want to (for example, a WebAPI-based conversational Bot solution:

![Predictive Schema (https://historicnbastats.blob.core.windows.net/images/predictive_schema.PNG)](https://historicnbastats.blob.core.windows.net/images/predictive_schema.PNG)

![Predictive Experiment(https://historicnbastats.blob.core.windows.net/images/predictive_experiment.PNG)](https://historicnbastats.blob.core.windows.net/images/predictive_experiment.PNG)

-----
### References
  [1]: Stats source http://www.basketball-reference.com/
  -----
  [2]: Blog post with similar experiment using NodeJS and Python... http://fabianbuentello.io/blog/NBA_Machine_Learning_Tutorial/
  -----
  [3]: ...and the related repo :) https://github.com/initFabian/NBA-Machine-Learning-Tutorial
  -----
