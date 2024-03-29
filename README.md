# Description
- 機能
  - 画像を、一定の縦横比を保ったままトリミングできます
  - 回転してトリミングすることもできます
- 開発の目的
  - 前回(MyTrimmingNew)の多数の反省点を受け、BDDを前提に再度開発したらどうなるかを確認するために再開発しました
- 前回からの改善点
  - 機能面
    - 矩形を回転している場合でも矩形を拡大/縮小可能にしました
    - 回転してトリミングする際、画像のジャギーを低減するロジックを実装しました
    - 回転してトリミングする際に進捗のプログレスバー表示を追加しました
  - 開発面
    - テスト側にロジックを2重実装しませんでした
    - 機能テスト＝受け入れテストと呼べるものを実装しました
- 注意事項
  - ~~回転してトリミングする際の処理は相変わらず遅いです~~
    - Ver. 1.1.0 にてある程度高速化しました。1920 x 1080 画像なら補正込みで 23 秒 -> 3 秒程度にしました
      - 高速化作業履歴は SpeedUp ブランチを参照ください
    - Ver. 1.1.1 にて更に高速化しました、1920 x 1080 画像なら補正込みで 3 秒 -> 0.5秒程度にしました
      - 高速化作業履歴は SpeedUp2 ブランチを参照ください
- 主な反省点
  - 全体的に
    - 本来あるべき上位レベルコメントが少なすぎた
      - コメントを記載したくなったときにその内容を極力コードで表現しようと心掛けた結果
      - Why や What をもっとコメントに残すべき
    - 母国語は英語ではないため、原著英語の参考書より多めにコメントを残した方がよい
  - テスト
    - 機能テストが多すぎてテストが遅い
      - 単体テストや結合テストでやるべきテストを機能テストにしてしまっているため
    - テストで機能を説明する意識に欠けた結果、コメントが少なすぎたりテストが構造化されていない
    - 1 テストで複数項目テストしていてテストが読みにくい
    - 外部と関係するテストは結合テストであることを理解していなかった
      - このため単体テストと結合テストがごっちゃになってる
  - プロダクト
    - CutLine の各種パラメーター受け渡し、パラメーター毎の基本データ型での受け渡しではなくもっとすっきりした方法なかったか？
      - CutLine クラスの各種パラメーター公開方法、1 パラメーターずつ public にするのはやり過ぎ
    - CutLine を操作する Command 周り、もっと単純にできる気がする
    - 画像処理
      - アルゴリズムを説明するコメントがない、組んだ本人にしかわからないことになってる
      - 高速化手法の説明がないので変更しにくい

# History
|日付      |タグ  |内容|
|---       |---   |--- |
|2024/ 1/ 5|v1.2.0|.NET Framework バージョンを 4.5 から 4.8 に更新|
|2021/10/16|v1.1.2|BitmapDataの色の並び順の間違いを修正|
|2021/10/13|v1.1.1|画像保存処理を更に高速化|
|2021/ 9/11|v1.1.0|見た目からテスト用情報を削除、画像保存処理の高速化|
|2021/ 6/16|v1.0.0|初回実装|

# Usage

## 基本的な流れ

```
画像ファイルオープン
↓
赤色の矩形を変更し、切り取り範囲を指定
↓
(プレビュー画面により、切り取り後にどのような見た目になるかを確認)
↓
別名保存により、切り取り範囲を画像として保存
```

## 切り取り範囲を示す赤枠矩形
### 操作
```
矩形の隅以外でのドラッグ＆ドロップ : 移動
矩形の隅でのドラッグ＆ドロップ     : 拡大/縮小
```

### 制約
- 矩形の回転は、-80度 〜 +80度程度に抑えてください
  - ソフト側で制約をかけることはTODOです

## キーボードショートカット
### ファイル関係
```
"Ctrl" + "o" : 画像ファイルオープン
"Ctrl" + "s" : 切り取り範囲を別名保存
```

### 切り取り範囲を示す赤枠矩形
```
";"                      : 時計回りに1度回転
"-"                      : 反時計回りに1度回転
"Shift" + ";"            : 時計回りに10度回転
"Shift" + "-"            : 反時計回りに10度回転
"カーソルキー"           : 1pixel単位での移動
"Shift" + "カーソルキー" : 10pixel単位での移動
"Ctrl" + "z"             : 取り消し
"Ctrl" + "y"             : やり直し
```

### その他
プレビュー機能(現時点の切り取り範囲で切り取ると、どんな見た目になるか)
```
"Ctrl" + "p" : プレビュー画面表示
```

# Install
コード公開のみ実施しています。お手数ですがご自身でビルドしてください。

# Requirement
- ビルドに必要な環境
  - Visual Studio
  - .NET Framework 4.8
- Nuget 取得Package
  - テスト
    - MSTest
      - MSTest.TestAdapter 2.1.2
      - MSTest.TestFramework 2.1.2
    - 自動化API
      - Codeer.Friendly 2.6.1
      - Codeer.Friendly.Windows 2.15.0
      - Codeer.Friendly.Windows.Grasp 2.12.0
      - RM.Friendly.WPFStandardControls 1.41.2

# Licence
- [MIT](https://github.com/tcnksm/tool/blob/master/LICENCE)
