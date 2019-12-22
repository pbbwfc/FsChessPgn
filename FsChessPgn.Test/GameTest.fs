#nowarn "25"
namespace FsChessPgn.Test

open FsChessPgn.Data

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type GameTest() =
    let testGame1 = "
[Event \"Breslau\"]
[Site \"Breslau\"]
[Date \"1879.??.??\"]
[Round \"?\"]
[White \"Tarrasch, Siegbert\"]
[Black \"Mendelsohn, J.\"]
[Result \"1-0\"]
[WhiteElo \"\"]
[BlackElo \"\"]
[ECO \"C49\"]

1.e4 e5 2.Nf3 Nc6 3.Nc3 Nf6 4.Bb5 Bb4 5.Nd5 Nxd5 6.exd5 Nd4 7.Ba4 b5 8.Nxd4 bxa4
9.Nf3 O-O 10.O-O d6 11.c3 Bc5 12.d4 exd4 13.Nxd4 Ba6 14.Re1 Bc4 15.Nc6 Qf6
16.Be3 Rfe8 17.Bxc5 Rxe1+ 18.Qxe1 dxc5 19.Qe4 Bb5 20.d6 Kf8 21.Ne7 Re8 22.Qxh7 Qxd6
23.Re1 Be2 24.Nf5  1-0"

    let testGame2= "
[Event \"Braingames WCC\"]
[Site \"London ENG\"]
[Date \"2000.11.02\"]
[Round \"15\"]
[White \"Kasparov,G\"]
[Black \"Kramnik,V\"]
[Result \"1/2-1/2\"]
[WhiteElo \"2849\"]
[BlackElo \"2770\"]
[ECO \"E05\"]

1.d4 Nf6 2.c4 e6 3.g3 d5 4.Bg2 Be7 5.Nf3 O-O 6.O-O dxc4 7.Qc2 a6 8.Qxc4 b5
9.Qc2 Bb7 10.Bd2 Be4 11.Qc1 Bb7 12.Bf4 Bd6 13.Nbd2 Nbd7 14.Nb3 Bd5 15.Rd1 Qe7
16.Ne5 Bxg2 17.Kxg2 Nd5 18.Nc6 Nxf4+ 19.Qxf4 Qe8 20.Qf3 e5 21.dxe5 Nxe5 22.Nxe5 Qxe5
23.Rd2 Rae8 24.e3 Re6 25.Rad1 Rf6 26.Qd5 Qe8 27.Rc1 g6 28.Rdc2 h5 29.Nd2 Rf5
30.Qe4 c5 31.Qxe8 Rxe8 32.e4 Rfe5 33.f4 R5e6 34.e5 Be7 35.b3 f6 36.Nf3 fxe5
37.Nxe5 Rd8 38.h4 Rd5  1/2-1/2"

    let testGame3= @"
[Event """"]
[Site ""Tilburg""]
[Date ""1982""]
[Round """"]
[White ""Huebner, R.""]
[Black ""Browne, W.""]
[Result ""1-0""]
[WhiteElo ""2630""]
[BlackElo ""2590""]
[ECO ""A30""]
[Opening ""English""]
[Variation ""symmetrical, hedgehog system""]
[Annotator ""Huebner, R.""]

1. Nf3 c5 2. c4 Nf6 3. Nc3 e6 4. g3 b6 5. Bg2 Bb7 6. O-O Be7 7. d4 cxd4 8.
Qxd4 d6 9. b3 Nbd7 10. Bb2 (10. e4 Nc5 $5) 10... a6 11. Rfd1 O-O 12. h3 Qc7
13. Qe3 Rfc8 (13... Rfe8 $5) 14. Nd4 Bxg2 15. Kxg2 Ne5 $11 16. Kg1 (16. f3
Qb7 17. a4 Ng6 18. Rac1 h5 { ($140 h4) } 19. h4 Ne5 $36) 16... Nc6 $6 (
16... h5 $6 17. f4 Nc6 (17... Ng6 18. f5 $16) 18. Nf3 $14) (16... Ng6 $6
17. f4 { $140 18.f5, 18.Qf3 }) (16... Qb7 $5 17. f4 $140 Nc6 18. Nf3 $6 b5)
17. Rac1 (17. Qf3 Nd7) 17... Qb7 (17... Nd7 $2 18. Nd5) 18. Nxc6 Rxc6 (
18... Qxc6 $2 19. Nd5 exd5 20. Qxe7 Re8 21. cxd5 Qxc1 22. Qxe8+ Rxe8 23.
Rxc1 Rxe2 (23... Nxd5 24. Rc6 $16) 24. Kf1 Re8 25. Bxf6 gxf6 26. Rc6 $16)
19. a4 Ne8 $6 (19... Rac8 $5 20. Ba3 Bf8 21. Qf3 $140 d5 22. Bxf8 Kxf8 23.
cxd5 Nxd5) 20. Ba3 (20. Ne4 $6 b5 $15) (20. Rd3 Bf6 { ($140 b5) } 21. Ba3
Bxc3 22. Rdxc3 Rac8 $11 (22... b5 23. cxb5 Rxc3 24. Qxc3 axb5 25. a5 $14))
20... Rac8 21. Rd3 R6c7 (21... d5 $6 22. cxd5 (22. Bxe7 $2 dxc4) 22... Rxc3
23. Rdxc3 Rxc3 24. Rxc3 Bxa3 25. dxe6 Bc5 (25... f6 26. Qd3 { $140 27. Qd7,
27.Qc4 }) 26. exf7+ Qxf7 27. Qf3 $16) 22. Rcd1 Qb8 $6 (22... d5 23. Bxe7
dxc4 24. Rd6 (24. bxc4 Rxe7 25. Rb1 Rxc4 26. Qxb6 Qc6 (26... Qc8 $2 27. Rd8
Qc6 28. Qxc6 Rxc6 29. Rbb8 f5 30. Rxe8+ Rxe8 31. Rxe8+ Kf7 32. Ra8 Rxc3 33.
Rxa6 $16) (26... Qxb6 27. Rxb6 Rec7 28. Rd8 Rc8 29. Rxc8 Rxc8 30. Ne4 $14)
27. Qxc6 Rxc6 28. Rb8 f5 29. Ra8 $14) 24... Nxd6 (24... cxb3 25. Rxb6 Qa8
26. Bb4 a5 27. Nb5 $18) (24... Rxe7 25. Rxb6 Qa8 26. b4 $14) 25. Bxd6 Rd7
$44) (22... h6 $5 { $140 d5 }) 23. Qd2 { $140 Ne4 } (23. h4 $5) 23... Bf8 (
23... d5 $2 24. Bxe7 dxc4 25. Rd7 $18) 24. Bb2 $6 (24. Ne4 $2 d5 25. Bxf8
dxe4 $19) (24. h4 $5 d5 $6 (24... h5 $5 25. Qg5 g6 26. Ne4 d5 (26... Rc6
27. Bb2 { $140 Nf6 }) (26... Be7 27. Qf4 Rd7 28. Rf3 $36) 27. Bxf8 dxe4 28.
Rd7 { $140 Qe7 }) (24... h6 25. h5 { $140 g4 }) 25. Bxf8 dxc4 26. Bxg7 $1 {
(Browne, W) } 26... cxd3 (26... Nxg7 27. bxc4 $16) 27. Be5 dxe2 28. Qxe2
$16) 24... h6 25. h4 Nf6 (25... h5 26. Ba3 { $140 27.Qg5 g6 28.Ne4 }) 26.
Qf4 $5 (26. Ba3 Ne8 27. h5 $14) 26... Qb7 27. Rf3 { $140 g4 } (27. g4 $6 h5
28. g5 Nd7 29. Ne4 Nc5 30. Nxc5 (30. Nf6+ $2 gxf6 31. Qxf6 e5 $19) 30...
bxc5 $15) 27... h5 28. Rfd3 Rc5 29. f3 (29. Ba3 Rf5 30. Qd2 Ng4 31. f3 Ne5
32. Re3 Rd8 { $140 g6 $17 }) 29... R5c7 (29... Rf5 30. Qd2 Nd7 $2 31. Ne4
Nc5 32. Nxd6 Bxd6 33. Rxd6 Nxb3 34. Qc3 $18) 30. Kg2 Qa8 31. Qg5 (31. e4 $6
Nd7) (31. Ba3 d5 32. Bxf8 Kxf8 33. Qd6+ Kg8 34. Qxb6 dxc4 35. bxc4 (35.
Qxc7 cxd3 36. Qe5 dxe2 37. Nxe2 Nd5 $15) 35... Rxc4 36. Rd8+ Kh7 $15) (31.
Qd2 d5 32. cxd5 Nxd5 (32... exd5 33. Qf4 $14) 33. Nxd5 Rc2 34. Qg5 exd5 $8
35. R1d2 $16) 31... Rc5 32. Qd2 R5c6 33. Ba3 Ne8 (33... d5 34. Bxf8 Kxf8
35. cxd5 exd5 36. Nxd5 Rc2 37. Qb4+) 34. Qe3 $6 (34. Qg5 d5 (34... g6 35.
Ne4 Qb8 (35... d5 36. Bxf8 dxe4 37. Rd7 exf3+ 38. exf3 Ng7 39. Qe7 Rxf8 40.
Rd8 $18) 36. Bb2 Bg7 37. Bxg7 Kxg7 38. Nxd6 Nxd6 39. Rxd6 Rxd6 40. Qe5+ $18
) 35. Bxf8 Kxf8 36. Qxh5 Kg8 37. Rd4 Nf6 38. Qg5 dxc4 39. b4 { $140 h5 $16
}) 34... g6 (34... Be7 $5) 35. Qd2 (35. Ne4 d5 36. Bxf8 dxe4 37. Rd7 Nf6
$17) (35. Qf4 d5 36. Bxf8 Kxf8 $11) 35... Qb8 $6 (35... Bg7 $2 36. Ne4 $18)
(35... R8c7 36. Ne4 d5 37. Bxf8 dxe4 38. Rd8 exf3+ 39. exf3 Rc8 40. Ba3 $16
) (35... R6c7 $5) 36. Qf4 Qa8 $2 (36... d5 37. Qxb8 Rxb8 38. cxd5 Rxc3 39.
Rxc3 Bxa3 40. dxe6 fxe6 41. Rc6 { $140 Rd7 $14 }) 37. Ne4 $16 e5 (37... d5
38. Bxf8 dxe4 39. Rd7 exf3+ 40. exf3 Nf6 41. Rxf7 e5 (41... Kxf7 42. Rd7+
Ke8 43. Re7+ Kd8 44. Qxf6 $18) 42. Rg7+ Kxf8 43. Qh6 $18) 38. Qe3 (38. Qd2
$2 d5 39. cxd5 Rc2 $19) 38... Qb8 39. Nc3 Nf6 40. Nd5 Nxd5 41. Rxd5 Re8 (
41... Rd8 42. R1d3 Re8 (42... Be7 $2 43. Qd2 Qc7 44. Qd1 Qb8 45. a5 bxa5
46. c5 $16)) 42. Qd3 Re6 43. e4 (43. a5 bxa5 44. Rxa5 Rb6 45. b4 (45. Rb1
Qb7 46. Qd5 Re7 $13) 45... Re7 46. c5 e4 47. fxe4 dxc5 $13) 43... Be7 44.
Kf2 (44. a5 $6 bxa5 45. Rxa5 Bd8) 44... Qf8 45. Ke2 f5 $6 (45... Qb8 $5 46.
Rg1 (46. Kd2 Bf8 47. Kc2 $2 b5) 46... Qf8 47. Bc1 Qg7 { $140 Rc8 }) 46. Bc1
fxe4 $6 (46... f4 47. gxf4 exf4 (47... Bxh4 48. f5 $16) 48. Rg1 Kh7 49. Qd2
(49. Rxh5+ $2 gxh5 50. Qd5 Rg6 51. Rxg6 Rc5 52. Qe6 Re5 $19) 49... Bxh4 50.
Qxf4 Qxf4 51. Bxf4 $16) (46... Rc8 $5 47. Be3 Rb8) 47. Qxe4 (47. fxe4 Rf6
48. Qe3 (48. Be3 Rf3) 48... Qf7 $13) 47... Qf5 (47... Rc8 48. Be3 Rb8 49.
g4 hxg4 50. fxg4 $16 Bxh4 51. g5 Bg3 52. Qg4 $18) 48. Be3 Kf7 (48... Qh3
49. Bf2 Rc8 (49... Qh2 50. Rg1 Rc8 51. Rdd1 Qh3 52. Bxb6) 50. Rg1 Rb8 51.
g4 Bxh4 (51... hxg4 52. Rxg4 Kf7 53. Rd1 $16) 52. gxh5 Bxf2 53. Rxg6+ Kf7
54. Rxe6 $16) 49. R5d3 Qxe4 50. fxe4 g5 $6 (50... Bd8 51. Bg5 Ke8 (51...
Bxg5 52. hxg5 Kg7 (52... Ke7 53. Rf1 Rc8 54. Rdf3 Kd7 55. Rf7+ Re7 56. R7f6
Rg8 (56... Rg7 57. Rf7+) 57. b4 { $140 58.Rd1 Re6 59.c5 }) 53. Rf1 a5 (
53... Re7 54. Rf6 Rd7 55. Re6 { $140 Re5 }) (53... Rc7 54. Rf6) (53... b5
54. axb5 axb5 55. cxb5 Rc5 56. Rd5 Rxd5 57. exd5 Re7 58. Kd3 Rc7 59. Rf6)
54. Rdf3 Rc7 (54... Re7 55. Rf6) 55. Rf8 Rb7 56. Rd8) (51... Be7 52. Rf1+
Ke8 53. Rdf3 { ($140 Rf7) } 53... b5 54. cxb5 axb5 55. Bxe7 Rxe7 56. Rf8+
Kd7 57. a5 $16) 52. Rf1) (50... Ke8 $5 51. Bg5 Rc7 52. Rf1 Bxg5 53. hxg5
Rf7 54. Rxf7 Kxf7 55. a5 (55. Rd5 Ke7 56. a5 Kd7) 55... bxa5 56. Rd5 Re7)
51. hxg5 Kg6 52. Rf1 Bxg5 53. Rf8 Bxe3 54. Rxe3 $6 { $140 Ref3 } ($142 54.
Kxe3 b5 (54... Rf6 55. Ra8 $1 (55. Rxf6+ Kxf6 56. Kf3 Kg5 57. Rd5 (57. Kg2
h4) (57. Rd2 b5) 57... h4 58. g4 h3 59. Kg3 h2 60. Rd1 b5 $13) (55. Rb8 Kg5
$13) 55... Rc7 (55... b5 56. cxb5 axb5 57. a5) (55... a5 56. Rb8 Kg5 (56...
Rf1 57. Rd8 Rf6 58. Rd1) 57. Rd1 Rg6 58. Kf3) 56. Rxa6 Rcf7 57. Kd2 (57.
Rxb6 Rf2 58. Rdxd6+ (58. Rbxd6+ $2 Kg5 $19) 58... Kg7 59. Rd7 Rf3+ $11)
57... Rf2+ 58. Kc3 R2f3 59. Rxb6 $18) 55. cxb5 axb5 56. a5 (56. axb5 Rc5
$13 57. Rd8 (57. Rd5 Rc3+) (57. b6 Rb5 58. Rb8 Kg7) 57... Rxb5 58. R8xd6
Rxd6 59. Rxd6+ Kg5 60. Rd3 Rb4) 56... b4 57. Rb8 Rc5 58. a6 Ra5 59. Ra8 (
59. Rb6 Kg5 60. Rdxd6 Rxd6 61. Rxd6 Ra3 62. Kd3 Rxb3+ 63. Kc4 Rxg3 64. Kxb4
Rg1 $13) 59... Kg7 60. Rd5 Ra3 61. Kd3 Rxb3+ 62. Kc4 Ra3 63. Rb5 $18) 54...
b5 $8 (54... Rf6 55. Rb8 Kg5 56. Rd3 { ($140 Ra8) } 56... Kg4 57. Rg8+ Kh3
58. g4+ Kh4 59. gxh5 Rf4 60. h6) 55. cxb5 axb5 56. a5 (56. axb5 $2 Rb6 $11
57. Ref3 Rxb5 58. Rg8+ Kh7 59. Rg5 Kh6 60. Rff5 Rxb3 61. Rxh5+ Kg7) 56...
b4 (56... Ra6 57. b4 $18) (56... Rc2+ 57. Kd3 Ra2 58. Ra8 { $140 Kc3-b4 })
57. Rd3 (57. Rb8 Ra6 58. Rb5 Re8 { $140 59... Rea8, 59...Rc8 }) 57... Rc5
$6 (57... Kg7 58. Rb8 Rc5 59. Rd5 Rc2+ 60. Rd2 Rc5 61. Ra2 Rc3 62. a6 Rxg3
63. Ra1 { $140 a7 }) (57... Ra6 58. Rd5 { $140 59.Rb8, 59. Kd3 $18 }) (
57... Rc2+ 58. Rd2 Rc3 (58... Rxd2+ 59. Kxd2 d5 60. Rb8 { $140 Rb6 $18 })
59. a6 Rc7 60. Ra2 $18) (57... Rf6 $5 58. Rxf6+ Kxf6 59. Rd1 Rc2+ (59...
Ke6 60. Ra1 Ra6 61. Kd3 d5 62. exd5+ Kxd5 63. Ke3 e4 64. Rh1 $18) 60. Rd2
Rc6 (60... Rc3 61. Rxd6+ Kg5 62. Rd3 Rc2+ 63. Kf3 Ra2 64. Rd5 Kf6 (64...
Ra3 65. Rxe5+ Kg6 66. Kf4 Rxb3 67. Rg5+ Kh6 68. Rb5 Ra3 69. e5 Kg6 70. Rb6+
Kf7 71. a6) 65. Rb5 Ra3 66. Rxb4 Rxa5 67. Kg2) 61. Ra2 Rc3 62. a6 Rxg3 63.
Ra4 Rg8 64. Rxb4 h4 65. a7 Ra8 66. Rb7 $18) 58. Ra8 (58. Rd5 $2 Rxd5 59.
exd5 Re7 60. Ra8 (60. Rb8 Ra7 61. Rb5 Kf5 62. Kd3 Kg4 63. Kc4 Kxg3 64. Kxb4
h4 65. Rb8 h3 66. Rg8+ Kf2 67. Rh8 Kg2 68. Ka4 h2 69. b4 h1=Q 70. Rxh1 Kxh1
71. b5 e4 72. b6 Re7 73. a6 e3 74. b7 e2 $19) (60. Rd8 Ra7 61. Rxd6+ Kf5
62. a6 Ke4 $11) 60... Kf5 61. Kd3 Rc7 62. a6 Rc3+ 63. Kd2 Ke4 64. Rd8 (64.
a7 Rc7 { $140 Kd5 }) 64... Rc7 65. Rxd6 Ra7 $11) (58. a6 $5 Ra5 59. Ra8 d5
$5 (59... Kg7 60. Rd5 Ra3 61. Rb5 Re7 62. Rb7 Kf6 { ($140 Ra6) } 63. Rxe7
Kxe7 64. a7 $18) (59... Kg5 60. Rd5 Ra3 61. Kd3 Rxb3+ 62. Kc4 Ra3 63. Kxb4
Ra1 64. Ra5 $18) 60. a7 Rea6 61. Rxd5 Ra2+ 62. Rd2 R2a3 63. Rd7 Kf6 64. Rb7
$16) 58... Rf6 (58... Rc2+ 59. Rd2 Rc3 60. a6 $18) (58... Kg7 59. Ra7+ (59.
a6 Rf6 60. a7 Ra5 61. Rf3 Re6 $13 62. Rff8 $2 Rxa7) 59... Kg6 (59... Kg8
60. a6 Ra5 61. Ra8+ Kg7 62. Rd5) 60. a6 Rf6 61. Rd7 Ra5 62. a7 Kg5 63. Rd5
Ra3 64. Rb5 (64. Kd3 Rf3+ $13) 64... Rf8 65. Rxb4 $18) 59. a6 Ra5 (59...
Kg7 60. Ra7+) (59... Kg5 60. Rg8+ Rg6 61. Rxg6+ Kxg6 62. Rxd6+) 60. Rf3
Ra2+ (60... Re6 61. Rff8 d5 62. a7 Rea6 63. Rg8+ Kh7 64. Rh8+ Kg7 65. Rag8+
$18) (60... Rxf3 61. Kxf3 Kg7 62. Kg2 Ra2+ 63. Kh3 Kh7 64. a7 Kg7 65. Kh4
Kg6 66. g4 $18) 61. Ke3 Rxf3+ 62. Kxf3 Kg7 63. a7 Kh7 64. Ke3 Ra3 65. Kd3
Rxb3+ 66. Kc4 Ra3 (66... Rxg3 67. Rb8 Ra3 68. a8=Q Rxa8 69. Rxa8 h4 70. Kd3
Kg6 71. Ke3 $18) 67. Kxb4 Ra1 68. Kb5 Kg7 69. Kc6 Ra2 (69... Ra6+ 70. Kb7
$18) (69... Kh7 70. Kxd6 Ra5 71. Ke6 Kg7 72. Kf5 $18) 70. Kxd6 Ra5 71. Ke6
Kh7 72. Kf6 Ra3 73. Kxe5 1-0"
    let testGame4 = @"
[Event ""London Chess Classic""]
[Site ""London""]
[Date ""2009.12.13""]
[Round ""5""]
[White ""Adams, Michael""]
[Black ""Short, Nigel""]
[Result ""1/2-1/2""]
[WhiteElo ""2698""]
[BlackElo ""2707""]
[ECO ""C80""]

1. e4 e5 2. Nf3 Nc6 3. Bb5 a6 4. Ba4 Nf6 5. O-O Nxe4 6. d4 b5 7. Bb3 d5 8.
dxe5 Be6 9. Nbd2 Nc5 10. c3 Nxb3 11. Nxb3 Be7 12. h3 O-O 13. Re1 a5 14. a4
bxa4 15. Rxa4 Qd7 16. Nbd4 Nxd4 17. cxd4 Rfb8 18. Bd2 Rxb2 19. Rxa5 Rxa5
20. Bxa5 Bf5 21. Bc3 Rb6 22. Kh2 h6 23. Qa1 Rg6 24. Bd2 Kh7 25. Qc3 Be4 26.
Qe3 Qf5 27. Qf4 Qxf4+ 28. Bxf4 c5 29. dxc5 Bxc5 30. Be3 d4 31. Bd2 Bd5 32.
Rc1 Ba7 33. Ra1 Bb6 34. Ra4 d3 35. Be3 Bc7 36. Bf4 Rc6 37. Rd4 Bxf3 38.
gxf3 Rc3 39. Kg2 g5 40. e6 Bb6 41. Rxd3 Rxd3 42. e7 Rd8 43. exd8=Q Bxd8
1/2-1/2"

    let gm1 = testGame1|>FsChessPgn.Games.ReadFromString|>List.head
    let gm2 = testGame2|>FsChessPgn.Games.ReadFromString|>List.head
    let gm3 = testGame3|>FsChessPgn.Games.ReadFromString|>List.head
    let gm4 = testGame4|>FsChessPgn.Games.ReadFromString|>List.head

    [<TestMethod>]
    member this.Game_should_accept_a_standard_pgn_game1() =
        let ngm1 = gm1|>Game.SetaMoves
        Assert.AreEqual(gm1.MoveText.Length,ngm1.MoveText.Length)
        let last = ngm1.MoveText|>List.rev|>List.tail|>List.head
        let (HalfMoveEntry(_,_,_,lastamv)) = last
        let lastfen = lastamv.Value.PostBrd|>Board.ToStr
        Assert.AreEqual("4rk2/p1p2ppQ/3q4/2p2N2/p7/2P5/PP2bPPP/4R1K1 b - - 3 24",lastfen)

    [<TestMethod>]
    member this.Game_should_accept_a_standard_pgn_game2() =
        let ngm2 = gm2|>Game.SetaMoves
        Assert.AreEqual(gm2.MoveText.Length,ngm2.MoveText.Length)
        let last = ngm2.MoveText|>List.rev|>List.tail|>List.head
        let (HalfMoveEntry(_,_,_,lastamv)) = last
        let lastfen = lastamv.Value.PostBrd|>Board.ToStr
        Assert.AreEqual("6k1/4b3/p3r1p1/1pprN2p/5P1P/1P4P1/P1R3K1/2R5 w - - 1 39",lastfen)

    [<TestMethod>]
    member this.Game_should_accept_a_standard_pgn_game3() =
        let ngm3 = gm3|>Game.SetaMoves
        Assert.AreEqual(gm3.MoveText.Length,ngm3.MoveText.Length)
        let last = ngm3.MoveText|>List.rev|>List.tail|>List.head
        let (HalfMoveEntry(_,_,_,lastamv)) = last
        let lastfen = lastamv.Value.PostBrd|>Board.ToStr
        Assert.AreEqual("R7/P6k/8/4K2p/4P3/r5P1/8/8 b - - 0 73",lastfen)

    [<TestMethod>]
    member this.Game_should_accept_a_standard_pgn_game4() =
        let ngm4 = gm4|>Game.SetaMoves
        Assert.AreEqual(gm4.MoveText.Length,ngm4.MoveText.Length)
        let last = ngm4.MoveText|>List.rev|>List.tail|>List.head
        let (HalfMoveEntry(_,_,_,lastamv)) = last
        let lastfen = lastamv.Value.PostBrd|>Board.ToStr
        Assert.AreEqual("3b4/5p1k/7p/6p1/5B2/5P1P/5PK1/8 w - - 0 44",lastfen)
