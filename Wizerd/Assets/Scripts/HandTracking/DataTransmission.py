import HandModule
import cv2
import socket

width, height = 640, 360

cap = cv2.VideoCapture(0)
cap.set(3, width)
cap.set(4, height)

hm = HandModule.HandModule(min_detection_confidence=0.5)

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
serverAddressPort = ("127.0.0.1", 2912)

chunk_size = 40000

# Configuration
main_hand = HandModule.HandType.LEFT_HAND.value
player_camera = False
hand_connections = True

while True:
    # Preprocessing ------------------------------------------------------------
    success, frame = cap.read()    
    frame = cv2.flip(frame, 1)
    frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
    # --------------------------------------------------------------------------
    
    # Processing ---------------------------------------------------------------
    hm.process(frame)
        
    # Sending landmarks data
    for hand_type in hm.hands_detected:        
        landmarks_data = []

        for landmark in hm.hand_landmarks[hand_type]:
            landmarks_data.extend([landmark.x, 1 - landmark.y, landmark.z])
            
        encoded_landmarks = b'L' + str.encode(hand_type[0], encoding="utf-8") + str.encode(str(landmarks_data), encoding="utf-8")
        sock.sendto(encoded_landmarks, serverAddressPort)
    
    # Draw hand connections
    if hand_connections:
        frame = hm.drawHandsConnection()
    
    # Sending frame data
    if player_camera:
        frame = cv2.cvtColor(frame, cv2.COLOR_RGB2BGR)
            
        _, encoded_frame = cv2.imencode('.jpg', frame)
        frame_data = encoded_frame.tobytes()
        
        for i in range(0, len(frame_data), chunk_size):
            end_idx = i + chunk_size
            chunk = b'F' + frame_data[i:end_idx] if end_idx >= len(frame_data) else b'C' + frame_data[i:end_idx]
            sock.sendto(chunk, serverAddressPort)
    else:
        sock.sendto(b'N', serverAddressPort)
    # --------------------------------------------------------------------------
    
    # Debugging ----------------------------------------------------------------
    # frame = cv2.cvtColor(frame, cv2.COLOR_RGB2BGR)
    # cv2.imshow("Player Camera", frame)
    # if cv2.waitKey(1) & 0xFF == ord('q'):
    #     break