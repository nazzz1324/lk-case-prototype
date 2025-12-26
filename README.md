# Информационная система оценки компетенций студента
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

### Backend

| Технология | Версия | Назначение |
| --- | --- | --- |
| **Платформа** | .NET 8.0 | Основная среда выполнения для серверного приложения. |
| **Фреймворк** | ASP.NET Core | 8.0 |
| **Язык** | C# | - |
| **Архитектура** | Clean Architecture / DDD | - |
| **Стиль API** | REST API | - |
| **ORM** | Microsoft.EntityFrameworkCore | 9.0.7 |
| **База данных** | PostgreSQL | - |
| **Маппинг** | AutoMapper | - |
| **Аутентификация** | Microsoft.AspNetCore.Authentication.JwtBearer | 8.0.20 |
| **API Документация** | Swashbuckle.AspNetCore | 6.4.0 |
| **Логирование** | Serilog.AspNetCore | 9.0.0 |
| **Версионирование** | Asp.Versioning.Mvc.ApiExplorer | 8.1.0 |

### Заглушки Back-end для Frontend

| Технология | Версия | Назначение |
| --- | --- | --- |
| **Node.js** | 18+ | Runtime для JavaScript |
| **Express** | 4.21.2 | Веб-фреймворк |
| **TypeScript** | 5.6.3 | Типизированный JavaScript |

---

## Структура Front-end части проекта

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

## Структура Back-end части проекта

```
/
├── Account.Domain/                  # 1. ДОМЕННЫЙ СЛОЙ (Ядро Бизнес-Логики)
│   ├── Account.Domain.csproj        # Файл проекта .NET
│
│   ├── DTO/                         # Объекты Передачи Данных (Data Transfer Objects)
│   │   ├── Competence/              # DTO для работы с компетенциями
│   │   │   ├── CompetenceDto.cs
│   │   │   ├── CompetencesDto.cs
│   │   │   └── CreateCompetenceDto.cs
│   │   │
│   │   ├── Discipline/              # DTO для работы с дисциплинами
│   │   │   ├── CreateDisciplineDto.cs
│   │   │   ├── DisciplineDto.cs
│   │   │   └── DisciplinesDto.cs
│   │   │
│   │   ├── Group/                   # DTO для работы с группами
│   │   │   ├── CreateGroupDto.cs
│   │   │   ├── GroupDto.cs
│   │   │   └── GroupsDto.cs
│   │   │
│   │   ├── Indicator/               # DTO для работы с индикаторами
│   │   │   ├── CreateIndicatorDto.cs
│   │   │   ├── IndicatorDto.cs
│   │   │   └── IndicatorsDto.cs
│   │   │
│   │   ├── ProfessionalRole/        # DTO для работы с профессиональными ролями
│   │   │   ├── CreateProfessionalRoleDto.cs
│   │   │   ├── ProfessionalRoleDto.cs
│   │   │   └── ProfessionalRolesDto.cs
│   │   │
│   │   ├── Role/                    # DTO для работы с ролями
│   │   │   ├── CreateRoleDto.cs
│   │   │   └── RoleDto.cs
│   │   │
│   │   ├── Student/                 # DTO для работы с данными студентов
│   │   │   ├── DisciplineIndicatorScoreDto.cs
│   │   │   ├── StudentCompetencesDto.cs
│   │   │   ├── StudentDisciplineScoresDto.cs
│   │   │   └── StudentDisciplinesDto.cs
│   │   │
│   │   ├── Teacher/                 # DTO для работы с данными преподавателей (оценивание)
│   │   │   ├── SaveScoresDto.cs
│   │   │   ├── ScoreItemDto.cs
│   │   │   ├── ScoringDataDto.cs
│   │   │   ├── ScoringFilterDto.cs
│   │   │   ├── StudentScoreDto.cs
│   │   │   ├── TeacherDisciplineDto.cs
│   │   │   └── TeacherIndicatorDto.cs
│   │   │
│   │   ├── User/                    # DTO для аутентификации и пользователей
│   │   │   ├── LoginUserDto.cs
│   │   │   ├── RegisterUserDto.cs
│   │   │   └── UserDto.cs
│   │   │
│   │   ├── UserRole/                # DTO для управления ролями пользователей
│   │   │   ├── DeleteUserRoleDto.cs
│   │   │   ├── UpdateUserRoleDto.cs
│   │   │   └── UserRoleDto.cs
│   │   │
│   │   └── TokenDto.cs              # DTO для передачи токена аутентификации
│
│   ├── Dictionaries/                # Словари и вспомогательные структуры
│   │   └── ErrorDictionary.cs       # Словарь для маппинга кодов ошибок
│
│   ├── Entity/                      # Сущности Домена (Domain Entities)
│   │   ├── AuthRoleEntities/        # Сущности, связанные с аутентификацией и ролями
│   │   │   ├── Role.cs
│   │   │   ├── User.cs
│   │   │   ├── UserRole.cs
│   │   │   └── UserToken.cs
│   │   │
│   │   ├── Competence.cs            # Сущность компетенции
│   │   ├── CompetenceProle.cs       # Связь компетенции и профессиональной роли
│   │   ├── CompetenceScore.cs       # Оценка компетенции
│   │   ├── Discipline.cs            # Сущность дисциплины
│   │   ├── DisciplineScore.cs       # Оценка дисциплины
│   │   ├── EducationForm.cs         # Форма обучения
│   │   ├── Faculty.cs               # Факультет
│   │   ├── Group.cs                 # Группа
│   │   ├── GroupDiscipline.cs       # Связь группы и дисциплины
│   │   ├── Indicator.cs             # Сущность индикатора
│   │   ├── IndicatorDiscipline.cs   # Связь индикатора и дисциплины
│   │   ├── IndicatorScore.cs        # Оценка индикатора
│   │   ├── ProfessionalRole.cs      # Профессиональная роль
│   │   ├── Student.cs               # Сущность студента
│   │   ├── Teacher.cs               # Сущность преподавателя
│   │   └── TeacherDiscipline.cs     # Связь преподавателя и дисциплины
│
│   ├── Enum/                        # Перечисления Домена
│   │   ├── Course.cs                # Курс обучения
│   │   ├── EnrollmentStatus.cs      # Статус зачисления
│   │   ├── ErrorCodes.cs            # Коды ошибок
│   │   └── Roles.cs                 # Роли пользователей
│
│   ├── Interfaces/                  # Контракты и Интерфейсы
│   │   ├── Databases/               # Интерфейсы для работы с БД
│   │   │   ├── IStateSaveChanges.cs # Интерфейс для сохранения изменений
│   │   │   └── IUnitOfWork.cs       # Паттерн Unit of Work
│   │   │
│   │   ├── Entites/                 # Интерфейсы для сущностей
│   │   │   └── IPeople.cs           # Интерфейс для сущностей "людей"
│   │   │
│   │   ├── Repositories/            # Интерфейсы Репозиториев
│   │   │   └── IBaseRepository.cs   # Базовый интерфейс репозитория
│   │   │
│   │   ├── Services/                # Интерфейсы Сервисов (Бизнес-логика)
│   │   │   ├── IAuthService.cs
│   │   │   ├── ICompetenceService.cs
│   │   │   ├── IDisciplineService.cs
│   │   │   ├── IGroupService.cs
│   │   │   ├── IIndicatorService.cs
│   │   │   ├── IProfessionalRoleService.cs
│   │   │   ├── IRoleService.cs
│   │   │   ├── IStudentService.cs
│   │   │   ├── ITeacherService.cs
│   │   │   └── ITokenService.cs
│   │   │
│   │   ├── Validations/             # Интерфейсы Валидаторов
│   │   │   └── IBaseValidator.cs    # Базовый интерфейс валидатора
│   │   │
│   │   ├── IAuditable.cs            # Интерфейс для сущностей с аудитом
│   │   └── IEntityID.cs             # Интерфейс для сущностей с ID
│
│   ├── Result/                      # Стандартизированные Объекты Результатов
│   │   ├── BaseResult.cs            # Базовый класс результата
│   │   ├── CollectionResult.cs      # Результат с коллекцией данных
│   │   └── ExceptionResult.cs       # Результат с информацией об исключении
│
│   └── Settings/                    # Настройки Домена
│       └── JwtSettings.cs           # Настройки JWT
│
└── Account.api/                     # 2. СЛОЙ ПРЕДСТАВЛЕНИЯ (API)
    ├── Account.api.csproj           # Файл проекта .NET
    ├── Account.api.http             # Файл для тестирования HTTP-запросов
    │
    ├── Controllers/                 # Контроллеры API
    │   ├── AuthController.cs        # Аутентификация
    │   ├── CompetenceController.cs  # Компетенции
    │   ├── DisciplineController.cs  # Дисциплины
    │   ├── GroupController.cs       # Группы
    │   ├── IndicatorController.cs   # Индикаторы
    │   ├── ProfessionalRoleController.cs # Профессиональные роли
    │   ├── RoleController.cs        # Роли
    │   ├── StudentController.cs     # Студенты
    │   ├── TeacherController.cs     # Преподаватели
    │   └── TokenController.cs       # Управление токенами
    │
    ├── Middlewares/                 # Промежуточное ПО
    │   └── ExceptionHandlingMiddleware.cs # Обработка исключений
    │
    ├── Properties/                  # Свойства проекта
    │   └── launchSettings.json      # Настройки запуска
    │
    ├── Program.cs                   # Точка входа приложения
    ├── Startup.cs                   # Конфигурация приложения
    ├── appsettings.Development.json # Конфигурация для разработки
    └── appsettings.json             # Основная конфигурация
```
---
