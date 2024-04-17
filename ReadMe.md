## TODO

~~- 좌표 체계가 혼동스러움. 일단 이거 정리하자.~~
~~- Connector 구현 하기~~
- 새로운 UI 구현.  
  ~~- Connector 테스트 하기, Dispose 관련해서 테스트 진행하기.~~    
  ~~- Connector 구현 후 PendingConnection 완성.~~    
  ~~- Node UI 초기 설계, Node 이동 구현 필요.(NodeEditor 확인)~~
- Node 이동 시 최종적인 위치 변경은 위치 변경 후 Invallid~ 호출 그리고 데이터 저장해야 함. 이 구현은 거의 마지막에 이루어 질듯.   
  ~~- Panning 구현 다시 정리하기.(DAGlynEditor 구현 및 정리)~~   
  ~~- Panning 관련하여 Canvas 의 사이즈가 고정되서 움직여서 이것에 대한 버그가 발생한다. 이것을 해결해야 한다.~~
- DirectProperty 좀더 연구.  
  ~~- Nodify 에서 Node 추가하는 소스 코드 한번 살펴본다.~~  
  ~~- Connector 연결 구현, 이동에 따른 연결점(Anchor) 의 업데이트 구현, PendingConnection 및 연결 완료되었을때 연결선 구현~~    
  ~~- Connector 연결 구현을 위해서 Connector 에서의 PendingConnectionEventArgs 이벤트 발생시키고, 핸들러를 DAGlynEditor 추가해서 처리해야함.~~  
  ~~- PendingConnectionEventArgs 코드 정리 및 수정해야함.~~  
  ~~- 위의 연결 끝내고 Dispose 처리 해야겠다. 계속 신경쓰인다. Dispose 테스트 진행할때,~~
- Unload 관련 해서 좀더 깊게 들어가 본다.
  ~~- default 값으로 new 해주는 값인 경우, 이걸 별도로 static 으로 만들어 놓는 거와 새롭게 new 해주는 거랑 어떤게 성능이 더 좋은지 파악해야 할 거 같다.~~  
  ~~- PendingConnection 을 Editor 의 axaml 에 넣는 방향으로 하는게 코드량이 줄어들 것 같다. PendingConnection 만들어주는 부분~~    
  ~~- 여러 Layer 잡아주는 것 구현.~~
- PendingConnection UI 개선
  ~~- Connection 연결선 Layer 만들어 주기 및 속성 추가. Connection 연결선 생성 루틴 개발~~    
  ~~- unit 테스트 에서 Dispse 관련 테스트 진행 중 dotMemory 등 사용방법 숙지가 필요 할듯하다. 계속 실폐 중~~
  ~~- 이벤트에 따라 Connection 추가하기~~  
  ~~- Panning 시 Connection 의 잔상이 남는 버그 처리해야함.~~
  ~~- 버그 수정~~  
  ~~- Node 에서 Anchor 2 개가 설정되어야 한다.(In/Out) 이게 Connector 의 Anchor 에 바인딩 하는 형태로 가야 한다.~~    
  ~~- StartNode 는 Anchor(Out) 하나 있고, EndNode 도 Anchor(In) 하나 있다.~~
  ~~- 대략적으로 완성했지만 버그 있음. 버그는 내일 고치자 (2/29) Unloaded 처리 부분도 생각하자.~~    
  ~~- Node 위치 변경 시, 최종적으로 해당 위치를 업데이트는 하는 부분을 넣어두고 이것을 토대로 Anchor 계산하는 코드를 적용하자.~~
- 위의 내용 구현 후 테스트 좀 잘하고 github 업데이트 하자. 된장.   
  ~~- 새로운 버그 Node 의 특정 위치에서 이동이 이상하게 나타남. 그리고 "invalid transform string" 라는 이상이 나타남.~~    
  ~~- Canvas 와 EditorCanvas, LayoutCanvas 의 Bound 를 살펴보기로 한다. 이거 정검한번 해야함. 일단 버그는 여기서 발생한 문제는 아니다.~~  
  ~~- 하지만, 위의 코드는 한번 살펴보고 Bound 문제등도 한번 정검해보자. 시간날때. 일단 지우지 않고 남겨둔다.~~  
  ~~- Avalonia.Controls.Primitives 에서 https://reference.avaloniaui.net/api/Avalonia.Controls.Primitives/Thumb/BE3BA1F0~~
  ~~- 위의 코드 살펴보고, node 의 drag 를 개선하자. 현재 마우스 고속으로 움직였을때 화면 튀는 현상 발생한다.~~  
  ~~- 여러 노드를 추가 하는 구문을 넣어야 함.~~      
  ~~- 노드의 삭제~~
- connection 의 삭제를 넣어야 함.
- Connection 을 고도화 해야 함. 지금은 그냥 선만 넣어둔 형태임.
- 노드를 생성하기 위한 UI 를 생각하자. 메뉴에 관한 스터디 및 어떻게 구현할지 고민해야 함.    
  ~~- 리소스 정리하자.~~    
  ~~- 1 차적으로 에디터에 배경 집어 넣는 것을 완성했는데, 추가적인 모양들을 넣어야 할 것 같다.~~
- Rider 에서 UI 보여주는 화면과 비슷하게 만들자. (시간날때, 기능상으로는 핵심 기능은 아님)
- node 가 focus 를 가질때 ui 변경 (구별해주어야 함.)
- compiled binding 적용 중 (잊어버리고 있었음.)
- https://docs.avaloniaui.net/docs/basics/data/data-binding/compiled-bindings

## 1차 버그 (4월 까지 이거 정리 및 TODO 정리, 그 후에 UX 들어 간다. 대략적인 구현은 끝남.)
~~- Panning 한다음에 선을 연결 시키면 이상하게 나옴. 드래그 할때는 잘못된 위치값을 가지다가, up 하면 정상적으로 선을 긋는다.~~
~~- 위의 문제는 PendingConnection 이라는 컨트롤이 panning 이 되지 않는 문제이다. 이건 xaml 구조의 문제인 듯 하다. Editor.xaml 살펴보기~~
- 노드 이동시 잔상 남는 현상 해결 해야 함.
- 선 연결시 미세한 버그 있음. 일단 문제는 해결 했는데, 지속적으로 살펴보자.
- 코드 수정이 많아서 안정화 되면 커밋한다. 일단 당분간 커밋 하지 않는다.
- 노드와 노드를 연결한 후 즉, 선이 연결된 후 노드를 이동 시킬때, 선의 잔상이 남는다.
- 당분간 코드 최적화 진행, 저녁 시간에는 UI/UX 스케치
  ~~- ContextMenu 처리할 때 panning 됨. 위치도 변함. -> 새로운 contextmenu 를 만들어서 처리 해야할 것 같다.~~
- Moved 관련 이벤트 소스 코드 최적화 관심을 가지고, 지속적으로 테스트 및 코드 최적화 진행한다.

## 지속적으로 TODO 정리하자.
## 벤치마킹
- Pro .NET Benchmarking 책 읽고 싶다. T.T
- https://github.com/dotnet/BenchmarkDotNet

# DAGlynEditor
DAGlynEditor: Avalonia 기반의 고급 에디터로, 방향성 비순환 그래프(DAG)를 활용해 데이터 흐름을 디자인하고 시각화합니다.
직관적인 노드 기반 GUI 상호작용에 최적화되어 효율적인 데이터 처리 워크플로우를 제공합니다.

DAGlynEditor: An advanced editor based on Avalonia, designed to visualize and design data flows using Directed Acyclic Graphs (DAG).   
It's optimized for intuitive node-based GUI interactions, providing an efficient data processing workflow.

## 현재 발생 문제점 및 개발 진행 사항
~~8/11/23 렌더링이 안되는 문제가 발생했다. WPF 에서는 Canvas 에서 잘 렌더링이 되었는데, Avalonia 에서의 Canvas 문제인지, 아니면 나의 코드가 문제인지 파악해야한다.~~  
~~8/12/23 렌더링이 안되는 문제는 Connection 의 문제점이라고 판단이 된다.~~      
~~8/12/23 초기화 문제 해결했다. 렌더링 되게 일단 완성했다. 아직 잠재적인 문제점이 있지만 일단 렌더링은 된다.~~
~~8/14/23 Node 바인딩 문제  
-- Connector 와 Connector 의 이름을 연결 시켜야 하는데, Compiled Binding 부분에서 잘 안됨. Binding 부분의 이해력 부족.  
-- ItemsControl 의 ItemSource 에 넣은 Item 의 내용을 ItemTemplate 에서 정의 한 ContentPresenter 에 이름으로 바인딩 시켜야 한다.~~  
~~8/19/23 ContentPresenter 바인딩이 안되는 문제가 발생했다.~~  
~~8/20/23 바인딩 문제는 해결했는데, Visual Tree 를 검사를 하면 DataTemplate 이하가 렌더링 되지 않는 문제가 발생한다.~~  
~~-- WPF 와 ContentPresenter 바인딩 하는 방식도 다르고, ContentPresenter 의 ContentTemplate 에 DataTemplate 을 적용하는 방식이 다른 것 같다.~~  
~~8/25/23 Connector 가 생기지 않는 문제 발생. xaml 구조의 문제인거 같다. 이거 원인을 파악하고 해결해야 한다.~~

~~8/13/23 Node 를 일차적으로 테스트용으로 개발 진행중~~     
~~-- UI 형태만 만들고, Editor 로 넘어간 후, Editor 완성 후 디자인적으로 신경쓴다.~~   
~~8/15/23 Node 에 들어가는 Connector 를 구현 시작.~~      
~~-- Anchor 의 위치값을 부모로 부터 가져오는 것을 작성해야 한다.~~  
~~-- Node 를 저장할 수 있는 Container 를 만들어야 하고, 이 Container 는 위치값을 가져야 한다.~~    
~~-- Container 를 독립적으로 만들지 생각해봐야 한다.~~    
~~-- 일단 Canvas 에 넣어서 테스트 한번 진행하고, Connector 를 담을 수 있고, 위치 값을 가지는 Container 를 제작한다.~~    
~~8/16/23 Connector 에서 이벤트들을 구현한다. 이 이벤트를 통해서 부모에서 핸들러를 구현해서 사용할 수 있도록 한다.~~  
~~8/21/23 Connection, Connector, ItemContainer 연결 문제~~  
~~-- 서로 다른 좌표 영역에서 연결시켜야 하고, Connector 와 ItemContainer 는 **Connection 을 담을 수 있는 클래스가 아니다**.~~  
~~-- 어떻게 바인딩 시킬지 고민해야 한다. 좀 어렵다.~~  
~~8/22/23 **Connector, ItemContainer, Connection 클래스가 각각 독립적으로 설계하고 싶다.** 계속 종속적으로 구현하게 되는데 이것도 어렵다.~~    
~~8/22/23 Node 에 Connector 연결~~  
~~-- InputConnector 추가, Connector 의 axaml 없애버리고, InputConnector 로 대체. 하지만 **이러면 다양한 모습을 할 수 없다라는 단점이 생김.**~~    
~~8/28/23 초기 Editor 구현~~    
~~8/29/23 VirtualCanvas 구현 필요 및 이를 통해서 OverlayLayer 재 구현 또는 새롭게 구현.~~  
~~## 현재 DAGlynEditor 를 따로 떼어내서 구현중에 있음.~~  
~~12/7/23 Connector 에서 Drag&Drop 에 대한 이벤트 이슈가 발생하였음.~~    
~~- OverlayLayer 로 Canvas 구현하였음.~~
- ItemsControl 연결 해서 위치 정확히 렌더링 되는지 확인 필요.
- 테스트 코드 삭제 필요 및 추가 기능 구현 세부화 할 필요 있음.  
  ~~- ViewPort 구현해야함.~~    
  ~~- Panning 구현 진행중 (10-18-23)~~  
  ~~- Paning 관련 버그 발견. 해결하지 못함. (10-24-23)~~    
  ~~- RX 방식으로 이벤트 처리 필요  (바로 시작. 전통적인 방식하니 잘 안됨.)~~  
  ~~- 마우스에 따라 Paning 기능 추가 구현 중 (10-22-23)~~    
  ~~- Animation 구현 (10-19-23)~~  
  ~~- EditorCanvas 의 사이즈 설정에 대한 부분 고민해야힘.(최우선을 해결 필요)~~        
  ~~- Zoom 기능 추가 구현 필요.~~    
  ~~- MultiGesture 기능 구현 11/4/23~~    
  ~~- State 패턴 적용 및 관련 이벤트 적용, MultiGesture 기능 구현 이후. 이때 Zoom, Paning 기능 추가 및 확인. (11/7/23)~~   
  ~~- Container 구현 및 Container 에 Node 연결, 그리고, 이때 작업을 편안하게 해주는 API 구현(11/13/23)~~  (다른 방안을 찾음.)
  ~~- 컨테이너 핸들링 (드레그, 생성 삭제 등) 구현 필요. (11/15/23)~~  
  ~~- DAGlyn 과 통합.~~  
  ~~- BoxValue.cs 삭제 예정~~
- State 구문 삭제 예정 (삭제했지만 아직 업데이트 않함)
- Gesture 관련 구문 변경 및 삭제 예정 (삭제했지만 아직 업데이트 않함)  
  ~~- 코드 정리시 위치에 대한 규칙 정할것~~    
  ~~- Node 관련 이미지 찾기 및 제작 (1차 완료, 업데이트전)~~
- Connector 코드 정리 및 수정  
  ~~- IScrollSnapPointsInfo 확인하기~~    
  ~~- 좌표 체계가 혼동스러움. 일단 이거 정리하자.~~
  ~~- Connector 구현 하기~~
- 새로운 UI 구현.  
  ~~- Connector 테스트 하기, Dispose 관련해서 테스트 진행하기.~~    
  ~~- Connector 구현 후 PendingConnection 완성.~~    
  ~~- Node UI 초기 설계, Node 이동 구현 필요.(NodeEditor 확인)~~
- Node 이동 시 최종적인 위치 변경은 위치 변경 후 Invallid~ 호출 그리고 데이터 저장해야 함. 이 구현은 거의 마지막에 이루어 질듯.   
  ~~- Panning 구현 다시 정리하기.(DAGlynEditor 구현 및 정리)~~   
  ~~- Panning 관련하여 Canvas 의 사이즈가 고정되서 움직여서 이것에 대한 버그가 발생한다. 이것을 해결해야 한다.~~
- DirectProperty 좀더 연구.
- Nodify 에서 Node 추가하는 소스 코드 한번 살펴본다.
- Connector 연결 구현, 이동에 따른 연결점(Anchor) 의 업데이트 구현, PendingConnection 및 연결 완료되었을때 연결선 구현
- Node 디자인 및 axaml 구현 -> 노드끼리 연결되는 부분 구현 -> Connection/PendingConnection 상세 구현 및 정리 (이후, 향후 개발 사항 삭제)

## 현재 개선해야할 부분에 대한 아이디어(그냥 막씀, 아래 문제 내용에 대한)
~~8/22/23 Node 에 Connector 연결~~  
~~-- InputConnector 추가, Connector 의 axaml 없애버리고, InputConnector 로 대체. 하지만 **이러면 다양한 모습을 할 수 없다라는 단점이 생김.**~~
```
이 문제 같은 경우는 pending 할때는 그냥 실선으로 중앙에서 나와서 중앙으로 연결 시키는 형태로 해도 될듯하다. 
그리고 연결이 완료 된 이후에는 connector 가 생겨서 쌓이는 형태로 가져가면 될듯하다.

즉, connector 로 연결 시키는 것이 아니라 부분에서 마우스 입력 받아서 처리하는 형태로 가 면될듯하다.
부분이라는 의미는 노드에서 한쪽 면을 차지하는 컨트롤로 이해하면 될듯하다.
```
## 개발 고려 사항
1. 컨트롤들간에 데이터 송신 및 데이터 수신은 이벤트로 처리한다.

## 향후 개발시 참고사항(그냥 기록용으로, 향후 삭제)
1. Toolbar 같은 경우는 AvaloniaEdit 을 한번 참고해본다.    
   ~~2. Connector 의 axaml 없애버리고, InputConnector 로 대체. 하지만 이러면 다양한 모습을 할 수 없다라는 단점이 생김. 이 문제를 향후 처리함.~~  
   ~~3. AvaloniaEdit Utils 에서 ExtensionMethods 일부와 FileReader 를 가지고 왔다. 향후 수정해서 사용한다.~~     
   ~~4. InputConnectorPanel 삭제 예정.~~    
   ~~5. Connector 추가/삭제는 Header 설정 창에서 설정.~~    
   ~~6. Connector 자체가 없어도 된다는 생각도 하고 있다. 하지만 그것은 추후 업데이트 하자. (하나로 하는 것ㅇ로 생각 변경. 계속 생각해보기)~~

## 미룬 것들 (머리 써야 하는 것들) TODO 확인 필요.
1. Zoom 기능 추가 구현
   ~~2. EditorCanvas 의 사이즈 설정에 대한 부분 고민해야힘.(최우선을 해결 필요)~~    
   ~~- 이건 각 모니터 사이즈에 따라 달리 설정하는 설정의 문제인 거 같다. 좀 생각을 단단히 해야 할 것 같다.~~
3. Connection.cs 에서 SourceOffset, TargetOffset 관련 값의 설정에 대한 제한을 걸어야 함.  
   ~~4. Location 에 대한 생각 및 구현을 구체화 해야 함. (PendingConnection 추가 및 완성 해야함. Connector 와 연관됨.)~~    
   ~~5. Connector 를 일단 원형으로 구현하고 있는데, 향후 직사학형 형태로 제작하고 Input/Output 을 각각 색상을 조정할 예정임.~~
6. 리소스 통합하기 ~~(PendingConnection.axaml 아직 추가 안됨.)~~
~~7. 업데이트 꼭 확인하기. 놓친거 있음. (부분적으로 업데이트 했는데 완전히 동일하게 할지 고민중)~~  

## 추가된 libs (Reactive 버전 잘 살펴야함.) 
- DagEdit 으로 이름을 바꿨는데 이전 프로젝트에서 설치한 테스트 하기 위한 테스트 프로젝트때문에 설치한 라이브러리 확인할 것
- Avalonia 11.0
- Avalonia.Desktop 11.0
- Avalonia.Diagnostics 11.0
- Avalonia.Fonts.Inter 11.0
- Avalonia.Themes.Fluent 11.0
- ReactiveUI 19.5.1
- System.Reactive 6.0

11.0 이상 버전에서 에러남 에러나는 버전은 11.0.7, 11.0.10 확인됨 그 이하 버전은 아직 확인하지 못함. 
일단, 이문제는 나중에 보고를 하던, 내 코드의 문제인지 판단 하기로 함. 일단 버그인거 같음.