### Excel 데이터를 JSON으로 변환하고, 게임에서 사용할 데이터로 로드하는 시스템입니다. 
---

## ExcelToJson

‘실행’ 버튼으로 개발자가 쉽게 unity editor에서 엑셀 파일을 JSON 파일로 변환할 수 있도록 구현했습니다. (CustomEditor스크립트 ExcelToJsonButton 사용) 

변환 실행 및 json파일의 경로를 설정합니다. 

 
## ExcelToJsonConverter 

Excel(‘.xlsx’) 파일을 읽어 json으로 변환합니다. 


## JsonParsing

JSON 파일을 파싱해 DataManager의 필드를 초기화합니다. 
