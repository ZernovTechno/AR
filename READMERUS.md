[ENGLISH README](./README.md)

# Привет! 
## Это мой AR-проект, работающий только на Android смартфоне. 
(и VR-очках)

Его написал на Unity один школьник, поэтому рекомендую вам воспользоваться этим руководством.

## Если вы хотите просто использовать его...
Выберите меню «Releases» в правой части экрана и загрузите APK-файл последней версии. Затем установите его на свой телефон и запустите. Интерфейс только на русском, на данный момент.

## Если вы хотите скомпилировать этот проект...

!!ИСПОЛЬЗУЙТЕ UNITY 2022.3.35f1, УСТАНОВЛЕННЫЙ С ANDROID BUILDING TOOLS!!
Импортируйте проект в Unity, открыв его из Unity Hub, затем подключите телефон с помощью USB-кабеля и включите на нем режим отладки USB. Разрешить установку приложений из ADB.

Установите конфигурацию Unity следующим образом (Она может быть установлена по умолчанию):

### Project Settings
### Player
#### Resolution and Presentation
![изображение](https://github.com/ZernovTechno/AR/assets/90546939/a37b0eda-85c2-4c09-a83c-4e5bcf3da646)

#### Other Settings
![изображение](https://github.com/ZernovTechno/AR/assets/90546939/6ccac38f-c521-406d-8782-dbe65974547b)

#### Publishing Settings
![изображение](https://github.com/ZernovTechno/AR/assets/90546939/07f3d81a-a2b9-4af5-9bde-126a721199a9)

А затем откройте сцену в проводнике внизу Unity. Сцена находится в разделе Assets->Samples->MediaPipe Unity Plugin->0.14.4->Official Solutions->Scenes->Hand Tracking->Hand Tracking.unity.

Затем нажмите File->Build Settings->Add Open Scenes, проверьте, что галочка на добавленной сцене стоит.

Теперь вы можете собрать его и запустить на телефоне. Проверьте подключение к телефону и нажмите "Build And Run" в меню «Build Settings/File menu».
