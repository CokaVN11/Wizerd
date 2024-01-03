import mediapipe as mp
from enum import Enum
import numpy as np

class HandType(Enum):
    LEFT_HAND = "Left"
    RIGHT_HAND = "Right"

class HandModule():
    def __init__(self, min_detection_confidence=0.5) -> None:        
        self.mp_hands = mp.solutions.hands
        self.hands = self.mp_hands.Hands(static_image_mode=False, min_detection_confidence=min_detection_confidence)
        self.tip_indices = [4, 8, 12, 16, 20]
        
        self.mp_draw = mp.solutions.drawing_utils
        
    def process(self, image):
        self.processing_image = image        
        self.results = self.hands.process(self.processing_image)        
        self.hands_detected = []        
        self.hand_landmarks = {
            HandType.LEFT_HAND.value: [],
            HandType.RIGHT_HAND.value: []
        }
        
        if self.results.multi_hand_landmarks:
            for hand_landmarks, hand_type in zip(self.results.multi_hand_landmarks, self.results.multi_handedness):
                hand_type = hand_type.classification[0].label

                self.hands_detected.append(hand_type)
                
                self.hand_landmarks[hand_type] = hand_landmarks.landmark
                    
    def getActiveFingers(self, hand_type):
        active_fingers = []
        
        # If hand_type existed
        if hand_type in self.hands_detected:
            angle = self.computeAngle(np.array([self.hand_landmarks[hand_type][0].x - self.hand_landmarks[hand_type][9].x, self.hand_landmarks[hand_type][0].y - self.hand_landmarks[hand_type][9].y]), np.array([1, 0]))
                        
            # If 50 < angle < 130
            if 0.873 < angle and angle < 2.269:
            
                # If up side
                if self.hand_landmarks[hand_type][0].y < self.hand_landmarks[hand_type][9].y:
                    
                    # Check thumb
                    if self.hand_landmarks[hand_type][1].x > self.hand_landmarks[hand_type][17].x:
                        active_fingers.append(self.hand_landmarks[hand_type][self.tip_indices[0]].x > self.hand_landmarks[hand_type][self.tip_indices[0] - 1].x)
                    else:
                        active_fingers.append(self.hand_landmarks[hand_type][self.tip_indices[0]].x < self.hand_landmarks[hand_type][self.tip_indices[0] - 1].x)
                        
                    # Check other fingers
                    for idx in range(1, 5):
                        active_fingers.append(self.hand_landmarks[hand_type][self.tip_indices[idx]].y > self.hand_landmarks[hand_type][self.tip_indices[idx] - 2].y)
                
                # If down side
                else:
                    
                    # Check thumb
                    if self.hand_landmarks[hand_type][1].x > self.hand_landmarks[hand_type][17].x:
                        active_fingers.append(self.hand_landmarks[hand_type][self.tip_indices[0]].x > self.hand_landmarks[hand_type][self.tip_indices[0] - 1].x)
                    else:
                        active_fingers.append(self.hand_landmarks[hand_type][self.tip_indices[0]].x < self.hand_landmarks[hand_type][self.tip_indices[0] - 1].x)
                        
                    # Check other fingers
                    for idx in range(1, 5):
                        active_fingers.append(self.hand_landmarks[hand_type][self.tip_indices[idx]].y < self.hand_landmarks[hand_type][self.tip_indices[idx] - 2].y)
                
            else:
                
                if self.hand_landmarks[hand_type][1].y > self.hand_landmarks[hand_type][17].y:
                    # Check thumb
                    active_fingers.append(self.hand_landmarks[hand_type][self.tip_indices[0]].y > self.hand_landmarks[hand_type][self.tip_indices[0] - 1].y)
                    
                    # Check other fingers
                    for idx in range(1, 5):
                        if self.hand_landmarks[hand_type][0].x > self.hand_landmarks[hand_type][9].x:
                            active_fingers.append(self.hand_landmarks[hand_type][self.tip_indices[idx]].x < self.hand_landmarks[hand_type][self.tip_indices[idx] - 2].x)
                        else:
                            active_fingers.append(self.hand_landmarks[hand_type][self.tip_indices[idx]].x > self.hand_landmarks[hand_type][self.tip_indices[idx] - 2].x)
                    
                else:
                    # Check thumb
                    active_fingers.append(self.hand_landmarks[hand_type][self.tip_indices[0]].y < self.hand_landmarks[hand_type][self.tip_indices[0] - 1].y)
                    
                    # Check other fingers
                    for idx in range(1, 5):
                        if self.hand_landmarks[hand_type][0].x > self.hand_landmarks[hand_type][9].x:
                            active_fingers.append(self.hand_landmarks[hand_type][self.tip_indices[idx]].x < self.hand_landmarks[hand_type][self.tip_indices[idx] - 2].x)
                        else:
                            active_fingers.append(self.hand_landmarks[hand_type][self.tip_indices[idx]].x > self.hand_landmarks[hand_type][self.tip_indices[idx] - 2].x)
        
        return active_fingers
    
    def computeAngle(self, v1, v2):
        # Normalize the vectors
        v1_u = v1 / np.linalg.norm(v1)
        v2_u = v2 / np.linalg.norm(v2)
        
        # Compute the angle (in radian) using the dot product
        angle = np.arccos(np.clip(np.dot(v1_u, v2_u), -1.0, 1.0))
        
        # Return angle (in radian)
        return angle
    
    def drawHandsConnection(self):
        image = self.processing_image

        if self.results.multi_hand_landmarks:
            for hand_landmarks in self.results.multi_hand_landmarks:
                self.mp_draw.draw_landmarks(image, hand_landmarks, self.mp_hands.HAND_CONNECTIONS)
                
        return image