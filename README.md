# Система оценки компетенций
***

## Cтек

### Frontend

| Технология | Версия | Назначение |
| --- | --- | --- |
| **React** | 19.0.0 | Библиотека для построения UI |
| **TypeScript** | 5.6.3 | Типизированный JavaScript |
| **Tailwind CSS** | 4.1.14 | Утилит-первый CSS фреймворк |
| **Vite** | 7.1.7 | Быстрый сборщик для разработки |
| **Wouter** | 3.3.5 | Маршрутизация для React |
| **shadcn/ui** | - | Компоненты UI на базе Radix |
| **Recharts** | 2.15.2 | Библиотека для графиков |
| **Framer Motion** | 12.23.22 | Анимации и переходы |
| **Lucide React** | 0.453.0 | Иконки SVG |

### Заглушки Back-end для Frontend

| Технология | Версия | Назначение |
| --- | --- | --- |
| **Node.js** | 18+ | Runtime для JavaScript |
| **Express** | 4.21.2 | Веб-фреймворк |
| **TypeScript** | 5.6.3 | Типизированный JavaScript |

---

## Структура fron-end части проекта

```
/
├── client/                          # Фронтенд приложение
│   ├── public/
│   │   ├── images/                 # Статические изображения
│   │   │   ├── admin_dashboard.png
│   │   │   ├── admin_users.png
│   │   │   ├── admin_disciplines.png
│   │   │   ├── admin_relations.png
│   │   │   ├── teacher_disciplines.png
│   │   │   ├── teacher_grading.png
│   │   │   ├── student_profile.png
│   │   │   ├── student_disciplines.png
│   │   │   ├── student_competencies.png
│   │   │   └── login_page.png
│   │   └── index.html              # HTML точка входа
│   │
│   ├── src/
│   │   ├── pages/                  # Страницы приложения
│   │   │   ├── Login.tsx           # Страница входа
│   │   │   ├── admin/
│   │   │   │   ├── Dashboard.tsx   # Дашборд администратора
│   │   │   │   ├── Users.tsx       # Управление пользователями
│   │   │   │   ├── Groups.tsx      # Управление группами
│   │   │   │   ├── Disciplines.tsx # Управление дисциплинами
│   │   │   │   ├── Competencies.tsx# Управление компетенциями
│   │   │   │   ├── Indicators.tsx  # Управление индикаторами
│   │   │   │   └── Relations.tsx   # Управление связями
│   │   │   ├── teacher/
│   │   │   │   ├── Disciplines.tsx # Дисциплины преподавателя
│   │   │   │   └── Grading.tsx     # Оценивание студентов
│   │   │   └── student/
│   │   │       ├── Profile.tsx     # Профиль студента
│   │   │       ├── Disciplines.tsx # Дисциплины студента
│   │   │       └── Competencies.tsx# Компетенции студента
│   │   │
│   │   ├── components/             # Переиспользуемые компоненты
│   │   │   ├── Sidebar.tsx         # Боковая панель
│   │   │   ├── Header.tsx          # Заголовок
│   │   │   ├── MetricCard.tsx      # Карточка метрики
│   │   │   ├── ui/                 # shadcn/ui компоненты
│   │   │   │   ├── button.tsx
│   │   │   │   ├── card.tsx
│   │   │   │   ├── dialog.tsx
│   │   │   │   ├── input.tsx
│   │   │   │   ├── select.tsx
│   │   │   │   ├── table.tsx
│   │   │   │   └── ... (другие компоненты)
│   │   │   └── ErrorBoundary.tsx   # Обработка ошибок
│   │   │
│   │   ├── lib/                    # Утилиты и помощники
│   │   │   ├── mockData.ts         # Демо-данные
│   │   │   └── utils.ts            # Вспомогательные функции
│   │   │
│   │   ├── contexts/               # React Context
│   │   │   └── ThemeContext.tsx    # Контекст темы
│   │   │
│   │   ├── hooks/                  # Пользовательские хуки
│   │   │   └── useAuth.ts          # Хук для авторизации
│   │   │
│   │   ├── App.tsx                 # Главный компонент с маршрутами
│   │   ├── main.tsx                # Точка входа React
│   │   └── index.css               # Глобальные стили и токены
│   │
│   └── tsconfig.json               # Конфигурация TypeScript
│
├── server/                          # Бэкенд приложение
│   └── index.ts                    # Express сервер
│
├── shared/                          # Общий код (типы)
│   └── const.ts                    # Константы
│
├── package.json                     # Зависимости проекта
├── tsconfig.json                    # Конфигурация TypeScript
├── vite.config.ts                   # Конфигурация Vite
├── tailwind.config.ts               # Конфигурация Tailwind
├── postcss.config.js                # Конфигурация PostCSS
└── prettier.config.js               # Конфигурация Prettier 
```

---

## Основной бек проекта

Допишите здесь своё, те кто делает бек.