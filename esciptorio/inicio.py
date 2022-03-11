
from tkinter import ttk
from tkinter import *
import tkinter as tk
import time
import cv2
import uuid
import os
import imutils
import numpy as np
import argparse
from firebase import firebase
import matplotlib.pyplot as plt
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import Dense, Dropout, Flatten
from tensorflow.keras.layers import Conv2D
from tensorflow.keras.optimizers import Adam
from tensorflow.keras.layers import MaxPooling2D
import threading
from tensorflow.keras.preprocessing.image import ImageDataGenerator
os.environ['TF_CPP_MIN_LOG_LEVEL'] = '2'
firebase = firebase.FirebaseApplication('https://linaemotional.firebaseio.com/', None)
def createNewWindowRegiter():
    newWindow = tk.Toplevel(app)
    newWindow.geometry(f"450x254+{screen_width-225}+{screen_height-127}")
    newWindow.resizable(0,0)
    newWindow.title("Registro De Usuario")
    global nombre
    global edad
    nombre=StringVar()
    edad=StringVar()
    panel_reg = PanedWindow(newWindow,orient=VERTICAL,relief="raised",bg="#1FC6CF")
    panel_reg.pack(fill=BOTH, expand=0)
    titulo=tk.Label(panel_reg,text="Registro De Usuario")
    titulo.config(fg="#080D0F",font=("Script MT Bold",28),bg="#1F88CF")
    panel_reg.add(titulo)
    panel_nom = PanedWindow(newWindow,bg="#1FC6CF")
    panel_nom.pack(fill=X, expand=0)
    label = Label(panel_nom, text="Nombre:")
    label.config(fg="#080D0F",font=("Script MT Bold",16),bg="#1FC6CF")
    panel_nom.add(label, padx=12, pady=12)
    entry = Entry(panel_nom, textvariable=nombre, validate="key", validatecommand=(newWindow.register(validate_entry_text), "%S"))
    entry.config(font=("Comic Sans MS",14))
    panel_nom.add(entry, padx=12, pady=12)
    panel_reg.add(panel_nom)
    panel_eda = PanedWindow(newWindow,bg="#1FC6CF")
    panel_eda.pack(fill=X, expand=0)
    label2 = Label(panel_eda, text="Edad:")
    label2.config(fg="#080D0F",font=("Script MT Bold",16),bg="#1FC6CF")
    panel_eda.add(label2, padx=12, pady=12)
    entry2 = Entry(panel_eda, textvariable=edad, validate="key", validatecommand=(newWindow.register(validate_entry_int), "%S"))
    entry2.config(font=("Comic Sans MS",14))
    panel_eda.add((Label(panel_eda, bg="#1FC6CF")), padx=7, pady=12)
    panel_eda.add(entry2, padx=12, pady=12)
    panel_reg.add(panel_eda)
    button_reg = tk.Button(panel_reg,text="Registrar",command=lambda:[newWindow.destroy(), registro()])
    button_reg.config(fg="#080D0F", font=("Script MT Bold",18), relief=tk.GROOVE, borderwidth=4, background="#11B2DE", activebackground="#2BD1FE")
    panel_reg.add(button_reg, padx=100, pady=12)
    newWindow.mainloop()

def validate_entry_int(text):
    return text.isdigit()

def validate_entry_text(text):
    return text.isalpha()

def validate_entry_text_report(text):
    return text.isalpha() or text=='.' or text==',' or text==' '

def registro():
    if 2 < len(nombre.get()) < 26 and 0 < len(edad.get()) < 4:
        createLibrary()
    else:
        messageBox("Datos Inválidos.")  
        
def createLibrary():
    personPath = dataPath + '/' + nombre.get() + '_' + edad.get()
    if not os.path.exists(personPath):
        os.makedirs(personPath)
    cap = cv2.VideoCapture(0,cv2.CAP_DSHOW)
    faceClassif = cv2.CascadeClassifier(cv2.data.haarcascades+'haarcascade_frontalface_default.xml')
    count = 0
    while True:
        ret, frame = cap.read()
        if ret == False: break
        frame =  imutils.resize(frame, width=640)
        gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
        auxFrame = frame.copy()
        faces = faceClassif.detectMultiScale(gray,1.3,5)
        for (x,y,w,h) in faces:
            cv2.rectangle(frame, (x,y),(x+w,y+h),(0,255,0),2)
            rostro = auxFrame[y:y+h,x:x+w]
            rostro = cv2.resize(rostro,(160,160),interpolation=cv2.INTER_CUBIC)
            cv2.imwrite(personPath + '/rostro_{}.jpg'.format(count),rostro)
            count = count + 1
        cv2.imshow('Reconocimiento Facial',frame)
        k =  cv2.waitKey(1)
        if k == 27 or count >= 300:
            break
    cap.release()
    cv2.destroyAllWindows()

    messageBox("Procesando...")

    peopleList = os.listdir(dataPath)
    labels = []
    facesData = []
    label = 0

    for nameDir in peopleList:
        personPath = dataPath + '/' + nameDir

        for fileName in os.listdir(personPath):
            labels.append(label)
            facesData.append(cv2.imread(personPath+'/'+fileName,0))
        label = label + 1

    face_recognizer = cv2.face.LBPHFaceRecognizer_create()
    face_recognizer.train(facesData, np.array(labels))
    face_recognizer.write('modeloLBPHFace.xml')
    messageBox("Proceso Terminado.")

def messageBox(msg):
    alert = tk.Toplevel(app)
    alert.geometry(f"250x122+{screen_width-125}+{screen_height-61}")
    alert.resizable(0,0)
    alert.title("Mensaje")
    panel_msg = PanedWindow(alert, orient=VERTICAL,relief="raised",bg="#1FC6CF")
    panel_msg.pack(fill=BOTH, expand=0)
    titulo = Label(panel_msg, text="")
    titulo.config(fg="#080D0F",font=("Script MT Bold",10),bg="#1F88CF")
    panel_msg.add(titulo)
    message = Label(panel_msg, text=msg)
    message.config(fg="#080D0F",font=("Script MT Bold",20),bg="#1FC6CF")
    panel_msg.add(message)
    button_ok = tk.Button(panel_msg,text="OK",fg="#080D0F",font=("Script MT Bold",14),command=alert.destroy, relief=tk.GROOVE, borderwidth=4, background="#11B2DE", activebackground="#2BD1FE")
    panel_msg.add(button_ok, padx=70, pady=5)
    alert.transient(app)
   
  

def createNewWindowinicsecio():
    imagePaths = os.listdir(dataPath)
    if len(imagePaths) == 0:
        messageBox("No Existen Registros.")
    else:
        face_recognizer = cv2.face.LBPHFaceRecognizer_create()
        face_recognizer.read('modeloLBPHFace.xml')
        cap = cv2.VideoCapture(0,cv2.CAP_DSHOW)
        faceClassif = cv2.CascadeClassifier(cv2.data.haarcascades+'haarcascade_frontalface_default.xml')
        count_des = 0
        contVerf = 0
        global datos
        while True:
            ret,frame = cap.read()
            if ret == False: break
            frame =  imutils.resize(frame, width=640)
            gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
            auxFrame = gray.copy()

            faces = faceClassif.detectMultiScale(gray,1.3,5)

            for (x,y,w,h) in faces:
                rostro = auxFrame[y:y+h,x:x+w]
                rostro = cv2.resize(rostro,(160,160),interpolation= cv2.INTER_CUBIC)
                result = face_recognizer.predict(rostro)

                if result[1] < 65:
                    cv2.putText(frame,'{}'.format(imagePaths[result[0]].split("_")[0]),(x,y-10),2,1.1,(0,255,0),1,cv2.LINE_AA)
                    cv2.rectangle(frame, (x,y),(x+w,y+h),(0,255,0),2)
                    contVerf=contVerf+1
                    datos=imagePaths[result[0]].split("_")
                    
                else:
                    cv2.putText(frame,'Desconocido',(x,y-10),2,0.8,(0,0,255),1,cv2.LINE_AA)
                    cv2.rectangle(frame, (x,y),(x+w,y+h),(0,0,255),2)
                    count_des = count_des + 1
                
            cv2.imshow('Inicio De Sesion',frame)
            k = cv2.waitKey(1)
            if k == 27 or contVerf == 150:
                break
            if count_des >= 300:
                messageBox("No Existe Usuario.")
                break

        cap.release()
        cv2.destroyAllWindows()
        if contVerf == 150:
            EmotionDetection()

def EmotionDetection():
    global Resulemotio
    Resulemotio=StringVar()
    model = Sequential()
    model.add(Conv2D(32, kernel_size=(3, 3), activation='relu', input_shape=(48,48,1)))
    model.add(Conv2D(64, kernel_size=(3, 3), activation='relu'))
    model.add(MaxPooling2D(pool_size=(2, 2)))
    model.add(Dropout(0.25))
    model.add(Conv2D(128, kernel_size=(3, 3), activation='relu'))
    model.add(MaxPooling2D(pool_size=(2, 2)))
    model.add(Conv2D(128, kernel_size=(3, 3), activation='relu'))
    model.add(MaxPooling2D(pool_size=(2, 2)))
    model.add(Dropout(0.25))
    model.add(Flatten())
    model.add(Dense(1024, activation='relu'))
    model.add(Dropout(0.5))
    model.add(Dense(7, activation='softmax'))

    model.load_weights('model.h5')
    cv2.ocl.setUseOpenCL(False)
    emotion_dict = {0: "ENOJADO", 1: "DISGUSTADO", 2: "TEMEROSO", 3: "FELIZ", 4: "NEUTRAL", 5: "TRISTE", 6: "SOPRENDIDO"}
    cap = cv2.VideoCapture(0)
    contVerf = 0
    while True:
        ret, frame = cap.read()
        if ret == False: break
        frame =  imutils.resize(frame, width=640)
        facecasc = cv2.CascadeClassifier(cv2.data.haarcascades+'haarcascade_frontalface_default.xml')
        gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
        faces = facecasc.detectMultiScale(gray,scaleFactor=1.3, minNeighbors=5)
        
        for (x, y, w, h) in faces:
            cv2.rectangle(frame, (x, y-50), (x+w, y+h+10), (255, 0, 0), 2)
            roi_gray = gray[y:y + h, x:x + w]
            cropped_img = np.expand_dims(np.expand_dims(cv2.resize(roi_gray, (48, 48)), -1), 0)
            prediction = model.predict(cropped_img)
            maxindex = int(np.argmax(prediction))
            Resulemotio=emotion_dict[maxindex]
            contVerf=contVerf+1
            cv2.putText(frame, datos[0], (x, y-90), cv2.FONT_HERSHEY_SIMPLEX, 1, (255, 0, 0), 2, cv2.LINE_AA)
            cv2.putText(frame, emotion_dict[maxindex], (x, y-60), cv2.FONT_HERSHEY_SIMPLEX, 1, (255, 0, 0), 2, cv2.LINE_AA)
        cv2.imshow('Detector De Estado De Animo',frame)
        k = cv2.waitKey(1)
        if k == 27 or contVerf == 150:
            break
    cap.release()
    cv2.destroyAllWindows()
    if contVerf == 150:
        hilo1 = threading.Thread(target = VewConsejo)
        hilo2 = threading.Thread(target = lambda:messageBox("Proscesando.."))
        hilo2.start() 
        hilo1.start()    
          

class Consejosdatos:
    def __init__(self,claveP,claveU,Descripsion,meGuta,nomeGusta,Habilitad,tiPoanimo,Edad):
        self.clavep=claveP
        self.claveu=claveU
        self.descripsion=Descripsion
        self.megustas=meGuta
        self.nomegusta=nomeGusta
        self.habilitado=Habilitad
        self.tipoanimo=tiPoanimo
        self.edad=Edad

def get_first(iterable, default=None):
    if iterable:
        for item in iterable:
            return item
    return default

def ConsejosNuevos():
    tolistConsejo = []
    result = firebase.get('/Publicacion',None)
    for clavePublicacion in result:
        resultdo = firebase.get('/Publicacion', clavePublicacion)
        TipoAnimo = resultdo.pop("TipoEstadoAnimo")
        Habilitado = resultdo.pop("Habilitado")
        Descriupcion = resultdo.pop("Descripcion")
        IdUsario = resultdo.pop("IdKeyU")
        MeGustas = resultdo.pop("Megusta")
        NoMegusta = resultdo.pop("NoMegusta")
        Edad = resultdo.pop("RangoEdad")
        rank=StringVar()
        if int (datos[1]) < 10:
            rank="Menor a 10"
        elif int (datos[1]) <= 18:
            rank="Entre 10 a 18"
        elif int (datos[1]) <= 30:
            rank="Entre 19 a 30"
        elif int (datos[1]) <= 50:
            rank="Entre 31 a 50"
        else:
            rank="Mayores a 50"

        if TipoAnimo==Resulemotio and Edad == rank and Habilitado=="1":           
            c=Consejosdatos(clavePublicacion,IdUsario,Descriupcion,MeGustas,NoMegusta,Habilitado,TipoAnimo,Edad)
            tolistConsejo.append(c)
    tolistConsejolink=sorted(tolistConsejo,key=lambda x:(x.nomegusta,x.megustas))
    ListNewConsejo =get_first(tolistConsejolink)
    return ListNewConsejo

def ConsejoRecomendado(): 
    tolistConsejoDos= []
    result = firebase.get('/Publicacion', None)
    for clavePublicacion in result:
        resultdo = firebase.get('/Publicacion', clavePublicacion)
        TipoAnimo = resultdo.pop("TipoEstadoAnimo")
        Habilitado = resultdo.pop("Habilitado")
        Descriupcion = resultdo.pop("Descripcion")
        IdUsario = resultdo.pop("IdKeyU")
        MeGustas = resultdo.pop("Megusta")
        NoMegusta = resultdo.pop("NoMegusta")
        Edad = resultdo.pop("RangoEdad")
        rank=StringVar()
        if int (datos[1]) < 10:
            rank="Menor a 10"
        elif int (datos[1]) <= 18:
            rank="Entre 10 a 18"
        elif int (datos[1]) <= 30:
            rank="Entre 19 a 30"
        elif int (datos[1]) <= 50:
            rank="Entre 31 a 50"
        else:
            rank="Mayores a 50"

        if TipoAnimo==Resulemotio and int(NoMegusta) < int(MeGustas) and Edad == rank and  Habilitado=="1":
            c=Consejosdatos(clavePublicacion,IdUsario,Descriupcion,MeGustas,NoMegusta,Habilitado,TipoAnimo,Edad)
            tolistConsejoDos.append(c)
    tolistConsejolink=[]
    tolistConsejolink=sorted(tolistConsejoDos,key=lambda x: x.megustas,reverse=True)
    ListNewConsejo =get_first(tolistConsejolink)
    return ListNewConsejo


def ConsejoMegusta(pConsejo):
    if pConsejo is None:
        pass
    else:
        cont=int(pConsejo.megustas)
        cont=cont+1
        firebase.put('/Publicacion',pConsejo.clavep,{'Descripcion':pConsejo.descripsion,'Habilitado':pConsejo.habilitado,'IdKeyU':pConsejo.claveu,'Megusta': str(cont),'NoMegusta':pConsejo.nomegusta,'RangoEdad':pConsejo.edad,'TipoEstadoAnimo':pConsejo.tipoanimo})
        pass
def vieConsejoRecomedado():
    newWindowCosejos = tk.Toplevel(app) 
    newWindowCosejos.resizable(0,0)
    vConsejo=ConsejoRecomendado()
    newWindowCosejos.title("Nuevo Consejo")
    panel_emo = PanedWindow(newWindowCosejos,orient=VERTICAL,relief="raised",bg="#1FC6CF")
    panel_emo.pack(fill=BOTH, expand=0)
    titulo = tk.Label(panel_emo,text=Resulemotio)
    titulo.config(fg="#080D0F",font=("Script MT Bold",28),bg="#1F88CF")
    panel_emo.add(titulo)
    if vConsejo is None:
        t="Disculpe las molestias pero no podemos brindarle otro consejo adecuado para su estado de ánimo."
        textoDelCosejo = tk.Label(panel_emo,text=t)
        textoDelCosejo.config(fg="#080D0F",font=("Script MT Bold",16),bg="#1FC6CF",wraplength=426)
        panel_emo.add(textoDelCosejo, padx=12, pady=12)
    else:
        t=vConsejo.descripsion
        textoDelCosejoD= tk.Label(panel_emo,text=t)
        textoDelCosejoD.config(fg="#080D0F",font=("Script MT Bold",16),bg="#1FC6CF",wraplength=426)
        panel_emo.add(textoDelCosejoD, padx=12, pady=12)
    Buttoncanselar = tk.Button(panel_emo,text="Aceptar",command=newWindowCosejos.destroy)
    Buttoncanselar.config(fg="#080D0F", font=("Script MT Bold",18), relief=tk.GROOVE, borderwidth=4, background="#11B2DE", activebackground="#2BD1FE")
    panel_emo.add(Buttoncanselar, padx=70, pady=8)
    panel_emo.update()
    newWindowCosejos.geometry(f"450x{panel_emo.winfo_height()}+{screen_width-225}+{screen_height-int(panel_emo.winfo_height() / 2)}")
    newWindowCosejos.mainloop()

def CosejoNoMegusta(pConsejo):
    
    cont=int(pConsejo.nomegusta)
    cont=cont+1
    firebase.put('/Publicacion',pConsejo.clavep,{'Descripcion':pConsejo.descripsion,'Habilitado':pConsejo.habilitado,'IdKeyU':pConsejo.claveu,'Megusta':pConsejo.megustas,'NoMegusta':str(cont),'RangoEdad':pConsejo.edad,'TipoEstadoAnimo':pConsejo.tipoanimo})
    hilo1 = threading.Thread(target = vieConsejoRecomedado)
    hilo1.start()

def ConfirmarReporte(pConsejo,Descripcion):
    d=StringVar()
    d=Descripcion
    firebase.put('/Publicacion',pConsejo.clavep,{'Descripcion':pConsejo.descripsion,'Habilitado':'0','IdKeyU':pConsejo.claveu,'Megusta':pConsejo.megustas,'NoMegusta':pConsejo.nomegusta,'RangoEdad':pConsejo.edad,'TipoEstadoAnimo':pConsejo.tipoanimo})
    firebase.post('/Reportes', data={"Descripcion":d,"IdKeyPublicasion":pConsejo.clavep})
 
    messageBox("Reporte fue enviado")

def validate_text_report(pConsejo, ptext, pView):
    if len(ptext) <= 500:
        pView.destroy()
        ConfirmarReporte(pConsejo, ptext)
    else:
        messageBox("Datos Inválidos.")

def ViewReportes(pCosejo):
    vCosejo=pCosejo
    newWindowReporte = tk.Toplevel(app)
    newWindowReporte.resizable(0,0)
    newWindowReporte.title("Reportar")
    panel_rep = PanedWindow(newWindowReporte,orient=VERTICAL,relief="raised",bg="#1FC6CF")
    panel_rep.pack(fill=BOTH, expand=0)
    titulo = tk.Label(panel_rep,text="Reportar")
    titulo.config(fg="#080D0F",font=("Script MT Bold",28),bg="#1F88CF")
    panel_rep.add(titulo)
    label = Label(panel_rep, text="Describir el motivo por el que consideras inadecuado el contenido:")
    label.config(fg="#080D0F",font=("Script MT Bold",16),bg="#1FC6CF",wraplength=426)
    panel_rep.add(label, padx=12, pady=12)
    reporte=StringVar()
    entry=Entry(panel_rep,textvariable=reporte, validate="key", validatecommand=(newWindowReporte.register(validate_entry_text_report), "%S"))
    entry.config(font=("Comic Sans MS",14))
    panel_rep.add(entry, padx=12, pady=0)
    panel_but = PanedWindow(panel_rep,bg="#1FC6CF")
    panel_but.pack(fill=X, expand=0)
    buttonncancelarr=Button(panel_but,text="Cancelar",command = newWindowReporte.destroy)
    buttonncancelarr.config(fg="#080D0F", font=("Script MT Bold",18), relief=tk.GROOVE, borderwidth=4, background="#11B2DE", activebackground="#2BD1FE")
    panel_but.add(buttonncancelarr, padx=12, pady=12, width=201)
    buttonnConfirmar=Button(panel_but,text="Confirmar",command= lambda : validate_text_report(vCosejo,reporte.get().strip(),newWindowReporte))
    buttonnConfirmar.config(fg="#080D0F", font=("Script MT Bold",18), relief=tk.GROOVE, borderwidth=4, background="#11B2DE", activebackground="#2BD1FE")
    panel_but.add(buttonnConfirmar, padx=12, pady=12, width=201)
    panel_rep.add(panel_but)
    panel_rep.update()
    newWindowReporte.geometry(f"450x{panel_rep.winfo_height()}+{screen_width-225}+{screen_height-int(panel_rep.winfo_height() / 2)}")
    newWindowReporte.mainloop()

def VewConsejo():
    newWindowCosejos = tk.Toplevel(app) 
    newWindowCosejos.resizable(0,0)
    vConsejo=ConsejosNuevos()
    newWindowCosejos.title("Consejo")
    panel_emo = PanedWindow(newWindowCosejos,orient=VERTICAL,relief="raised",bg="#1FC6CF")
    panel_emo.pack(fill=BOTH, expand=0)
    titulo = tk.Label(panel_emo,text=Resulemotio)
    titulo.config(fg="#080D0F",font=("Script MT Bold",28),bg="#1F88CF")
    panel_emo.add(titulo)
    t=StringVar()
    if vConsejo is None:
        t="Disculpe las molestias pero no podemos brindarle un consejo adecuado para su estado de ánimo."
        textoDelCosejo = tk.Label(panel_emo,text=t)
        textoDelCosejo.config(fg="#080D0F",font=("Script MT Bold",16),bg="#1FC6CF",wraplength=426)
        panel_emo.add(textoDelCosejo, padx=12, pady=12)
        Buttoncanselar = tk.Button(panel_emo,text="Aceptar",command=newWindowCosejos.destroy)
        Buttoncanselar.config(fg="#080D0F", font=("Script MT Bold",18), relief=tk.GROOVE, borderwidth=4, background="#11B2DE", activebackground="#2BD1FE")
        panel_emo.add(Buttoncanselar, padx=70, pady=8)
        panel_emo.update()
        newWindowCosejos.geometry(f"450x{panel_emo.winfo_height()}+{screen_width-225}+{screen_height-int(panel_emo.winfo_height() / 2)}")
        newWindowCosejos.mainloop()
    else:
        t=vConsejo.descripsion
        textoDelCosejoD = tk.Label(panel_emo,text=t)
        textoDelCosejoD.config(fg="#080D0F",font=("Script MT Bold",16),bg="#1FC6CF",wraplength=426)
        panel_emo.add(textoDelCosejoD, padx=12, pady=12)
        panel_but = PanedWindow(panel_emo,bg="#1FC6CF")
        panel_but.pack(fill=X, expand=0)
        ButtonMEgusta = tk.Button(panel_but,text="Me Gusta",command=lambda: [newWindowCosejos.destroy(), ConsejoMegusta(vConsejo)])
        ButtonMEgusta.config(fg="#080D0F", font=("Script MT Bold",18), relief=tk.GROOVE, borderwidth=4, background="#11B2DE", activebackground="#2BD1FE")
        panel_but.add(ButtonMEgusta, padx=12, pady=3, width=201)
        ButtonNoMEgusta = tk.Button(panel_but,text="No Me Gusta",command=lambda: [newWindowCosejos.destroy(), CosejoNoMegusta(vConsejo)])
        ButtonNoMEgusta.config(fg="#080D0F", font=("Script MT Bold",18), relief=tk.GROOVE, borderwidth=4, background="#11B2DE", activebackground="#2BD1FE")
        panel_but.add(ButtonNoMEgusta, padx=12, pady=3, width=201)
        panel_emo.add(panel_but)
        ButtonReport = tk.Button(panel_emo,text="Reportar",command=lambda: [newWindowCosejos.destroy(), ViewReportes(vConsejo)])
        ButtonReport.config(fg="#080D0F", font=("Script MT Bold",14), relief=tk.GROOVE, borderwidth=0, background="#1FC6CF", activebackground="#1FC6CF")
        panel_emo.add(ButtonReport, padx=12, pady=0)
        panel_emo.update()
        newWindowCosejos.geometry(f"450x{panel_emo.winfo_height()}+{screen_width-225}+{screen_height-int(panel_emo.winfo_height() / 2)}")
        newWindowCosejos.mainloop()

wd = os.getcwd()
dataPath = wd + '/Data'
if not os.path.exists(dataPath):
	os.makedirs(dataPath)

app = tk.Tk()
screen_width=app.winfo_screenwidth()
screen_width=int(screen_width/2)
screen_height=app.winfo_screenheight()
screen_height=int(screen_height/2)
app.geometry(f"350x257+{screen_width-175}+{screen_height-128}")
app.resizable(0,0)
app.title("Emotion Lina Escritorio")
panel = PanedWindow(orient=VERTICAL,relief="raised",bg="#1FC6CF")
panel.pack(fill=BOTH, expand=0)
titulo = Label(panel, text="Inicio")
titulo.config(fg="#080D0F",font=("Script MT Bold",40),bg="#1F88CF")
panel.add(titulo)

buttonRegistro = tk.Button(panel, 
              text="Registrarse",
              command=createNewWindowRegiter)
buttonRegistro.config(fg="#080D0F", font=("Script MT Bold",18), relief=tk.GROOVE, borderwidth=4, background="#11B2DE", activebackground="#2BD1FE")
panel.add(buttonRegistro, pady=15, padx=35)

buttonIniciarSecion = tk.Button(panel, 
              text="Iniciar Sesión",
              command=createNewWindowinicsecio)
buttonIniciarSecion.config(fg="#080D0F", font=("Script MT Bold",18), relief=tk.GROOVE, borderwidth=4, background="#11B2DE", activebackground="#2BD1FE")
panel.add(buttonIniciarSecion, pady=20, padx=35)

app.mainloop()