# smart-shopping-cart
This is a shopping cart application that demos how to enpower user experence by utilize computer vision and active learning. The vision model was trained by Azure CustomVision and exported as a Tensorflow.js model.

The application is written in Vue3. To improve accuracy and counter drifting, an active learning style is adoped. Everytime a manually checkin item added, it can be saved for further training a new model version. Training image can be augmented with rotation and translation.


