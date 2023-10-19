import cv2
import mediapipe as mp
import numpy as np
import socket

# 서버 설정
HOST = '127.0.0.1'  # 서버의 IP 주소
PORT = 12345  # 사용할 포트 번호

server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.bind((HOST, PORT))
server_socket.listen()

print(f'Server is listening on {HOST}:{PORT}')

client_socket, client_address = server_socket.accept()
print(f'Connected by {client_address}')

# mideapipe
mp_drawing = mp.solutions.drawing_utils
mp_drawing_styles = mp.solutions.drawing_styles
mp_pose = mp.solutions.pose
mp_face_mesh = mp.solutions.face_mesh

# 위치 정보
pose_ind = [11, 12, 13, 14, 15, 16, 23, 24, 25, 26, 27, 28]         # 몸통
face_ind = [0]                                                      # 얼굴

pose_name = ['LS','RS','LE','RE','LW','RW','LP','RP','LK','RK','LA','RA','P']
# 왼쪽 어깨(LS), 오른쪽 어깨(RS), 왼쪽 팔꿈치(LE), 오른쪽 팔꿈치(RE), 왼쪽 손목(LW), 오른쪽 손목(RW),
# 왼쪽 골반(LP), 오른쪽 골반(RP), 왼쪽 무릎(LK), 오른쪽 무릎(RK), 왼똑 발목(LA), 오른쪽 발목(RA), 인중(P)


## 영상촬영

print("영상촬영시작")

cap = cv2.VideoCapture(0)

## 소켓 통신
while True:
    ret, image = cap.read()
    if not ret:
        print("카메라를 찾을 수 없습니다.")

        # 동영상을 불러올 경우는 'continue' 대신 'break'를 사용합니다.
        break

    with mp_pose.Pose(
            min_detection_confidence=0.5,
            min_tracking_confidence=0.5) as pose:

        # 필요에 따라 성능 향상을 위해 이미지 작성을 불가능함으로 기본 설정합니다.
        image.flags.writeable = False
        image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
        results = pose.process(image)
        # print(results.pose_landmarks.landmark[0])

        # 포즈 주석을 이미지 위에 그립니다.
        image.flags.writeable = True
        image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)

        # 포즈 좌표 추출하기
        pose_info = []
        with mp_pose.Pose() as pose:
            # 필요에 따라 성능 향상을 위해 이미지 작성을 불가능함으로 기본 설정합니다.
            image.flags.writeable = False
            image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
            # print(results.pose_landmarks.landmark[0])

            # 포즈 주석을 이미지 위에 그립니다.
            image.flags.writeable = True
            image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)

            results = pose.process(image)
            if results.pose_landmarks:
                for i, point in enumerate(results.pose_landmarks.landmark):
                    if i in pose_ind:
                        pose_info.append([point.x, point.y, point.z])

                with mp_face_mesh.FaceMesh() as face_mesh:
                    results = face_mesh.process(image)

                    if results.multi_face_landmarks:
                        for person in results.multi_face_landmarks:
                            for i, point in enumerate(person.landmark):
                                if i in face_ind:
                                    pose_info.append([point.x, point.y, point.z])

                    else:
                        pose_info.append(None)

                pose_dict = dict(zip(pose_name, pose_info))
                # print(pose_dict)

    # 유니티에게 값을 데이터를 보냄
    data = client_socket.recv(1024)
    if not data:
        break
    else:
        # print(f'Received from Unity: {data.decode()}') #유니티에서 보낸 데이터를 파이썬에서 확인하는 코드
        client_socket.sendall(str(pose_dict).encode())  # new_list라는 변수가 유니티에 보내짐

cap.release()
client_socket.close()