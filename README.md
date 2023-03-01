# smart-shopping-cart
This is a shopping cart application that demos how to enpower user experence by utilize computer vision during checking out.
Once products put on counter, the CV will identify SKUs and have items list on the POS terminal in one shot. 
This supposed to be more efficent than scanning item one by one.

Active learning is utilized to counter drift. Since each location, POS have different light condition,  setup, it is necessary to have the 
training data collected individually at live enviroment and feed to training. Every time there is a miss labeling or no-detect at the checking out,
a cashed can manually correct it by manually Box the item on screen and link to a product SKU. This process is untimately a data labeling. 
The data can be stored and used to train next version of model. This way we same the training and operation cost.
This demo is using Azure CustomVision for training. The resulted model is exported as a Tensorflow.js model, then deployed to the edge device.



