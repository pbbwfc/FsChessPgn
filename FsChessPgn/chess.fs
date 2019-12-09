namespace FsChess

open System

module chess =
    let FILE_NAMES = ["a"; "b"; "c"; "d"; "e"; "f"; "g"; "h"]
    
    let RANK_NAMES = ["1"; "2"; "3"; "4"; "5"; "6"; "7"; "8"]

    type Square = int
    let A1, B1, C1, D1, E1, F1, G1, H1 :Square * Square * Square * Square * Square * Square * Square * Square =  0,1,2,3,4,5,6,7
    let A2, B2, C2, D2, E2, F2, G2, H2 = A1+8, B1+8, C1+8, D1+8, E1+8, F1+8, G1+8, H1+8 
    let A3, B3, C3, D3, E3, F3, G3, H3 = A2+8, B2+8, C2+8, D2+8, E2+8, F2+8, G2+8, H2+8 
    let A4, B4, C4, D4, E4, F4, G4, H4 = A3+8, B3+8, C3+8, D3+8, E3+8, F3+8, G3+8, H3+8 
    let A5, B5, C5, D5, E5, F5, G5, H5 = A4+8, B4+8, C4+8, D4+8, E4+8, F4+8, G4+8, H4+8 
    let A6, B6, C6, D6, E6, F6, G6, H6 = A5+8, B5+8, C5+8, D5+8, E5+8, F5+8, G5+8, H5+8 
    let A7, B7, C7, D7, E7, F7, G7, H7 = A6+8, B6+8, C6+8, D6+8, E6+8, F6+8, G6+8, H6+8 
    let A8, B8, C8, D8, E8, F8, G8, H8 = A7+8, B7+8, C7+8, D7+8, E7+8, F7+8, G7+8, H7+8 
    let SQUARES = [
        A1; B1; C1; D1; E1; F1; G1; H1;
        A2; B2; C2; D2; E2; F2; G2; H2;
        A3; B3; C3; D3; E3; F3; G3; H3;
        A4; B4; C4; D4; E4; F4; G4; H4;
        A5; B5; C5; D5; E5; F5; G5; H5;
        A6; B6; C6; D6; E6; F6; G6; H6;
        A7; B7; C7; D7; E7; F7; G7; H7;
        A8; B8; C8; D8; E8; F8; G8; H8
        ] 
    let SQUARE_NAMES = [for f in FILE_NAMES do for r in RANK_NAMES -> f+r]

    ///Gets a square number by file and rank index.
    let square(file_index: int, rank_index: int) =
        rank_index * 8 + file_index

    ///Gets the file index of the square where ``0`` is the a-file.
    let square_file(square: Square) =
        square &&& 7

    ///Gets the rank index of the square where ``0`` is the first rank.
    let square_rank(square: Square) =
        square >>> 3

    ///Gets the name of the square, like ``a3``.
    let square_name(square: Square) =
        SQUARE_NAMES.[square]

    ///Gets the distance (i.e., the number of king steps) from square *a* to *b*.
    let square_distance(a: Square, b: Square) =
        max(abs(square_file(a) - square_file(b)), abs(square_rank(a) - square_rank(b)))

    ///Mirrors the square vertically.
    let square_mirror(square: Square) =
        square ^^^ 0x38

    let SQUARES_180 = [for sq in SQUARES -> square_mirror(sq)]

    type Bitboard = uint64
    let BB_EMPTY:Bitboard = 0UL
    let BB_ALL:Bitboard = 0xffffffffffffffffUL

    let BB_A1, BB_B1, BB_C1, BB_D1, BB_E1, BB_F1, BB_G1, BB_H1 :Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard =  1UL <<< A1, 1UL <<< B1, 1UL <<< C1, 1UL <<< D1, 1UL <<< E1, 1UL <<< F1, 1UL <<< G1, 1UL <<< H1
    let BB_A2, BB_B2, BB_C2, BB_D2, BB_E2, BB_F2, BB_G2, BB_H2 :Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard =  1UL <<< A2, 1UL <<< B2, 1UL <<< C2, 1UL <<< D2, 1UL <<< E2, 1UL <<< F2, 1UL <<< G2, 1UL <<< H2
    let BB_A3, BB_B3, BB_C3, BB_D3, BB_E3, BB_F3, BB_G3, BB_H3 :Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard =  1UL <<< A3, 1UL <<< B3, 1UL <<< C3, 1UL <<< D3, 1UL <<< E3, 1UL <<< F3, 1UL <<< G3, 1UL <<< H3
    let BB_A4, BB_B4, BB_C4, BB_D4, BB_E4, BB_F4, BB_G4, BB_H4 :Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard =  1UL <<< A4, 1UL <<< B4, 1UL <<< C4, 1UL <<< D4, 1UL <<< E4, 1UL <<< F4, 1UL <<< G4, 1UL <<< H4
    let BB_A5, BB_B5, BB_C5, BB_D5, BB_E5, BB_F5, BB_G5, BB_H5 :Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard =  1UL <<< A5, 1UL <<< B5, 1UL <<< C5, 1UL <<< D5, 1UL <<< E5, 1UL <<< F5, 1UL <<< G5, 1UL <<< H5
    let BB_A6, BB_B6, BB_C6, BB_D6, BB_E6, BB_F6, BB_G6, BB_H6 :Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard =  1UL <<< A6, 1UL <<< B6, 1UL <<< C6, 1UL <<< D6, 1UL <<< E6, 1UL <<< F6, 1UL <<< G6, 1UL <<< H6
    let BB_A7, BB_B7, BB_C7, BB_D7, BB_E7, BB_F7, BB_G7, BB_H7 :Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard =  1UL <<< A7, 1UL <<< B7, 1UL <<< C7, 1UL <<< D7, 1UL <<< E7, 1UL <<< F7, 1UL <<< G7, 1UL <<< H7
    let BB_A8, BB_B8, BB_C8, BB_D8, BB_E8, BB_F8, BB_G8, BB_H8 :Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard =  1UL <<< A8, 1UL <<< B8, 1UL <<< C8, 1UL <<< D8, 1UL <<< E8, 1UL <<< F8, 1UL <<< G8, 1UL <<< H8
    let BB_SQUARES = [
        BB_A1; BB_B1; BB_C1; BB_D1; BB_E1; BB_F1; BB_G1; BB_H1;
        BB_A2; BB_B2; BB_C2; BB_D2; BB_E2; BB_F2; BB_G2; BB_H2;
        BB_A3; BB_B3; BB_C3; BB_D3; BB_E3; BB_F3; BB_G3; BB_H3;
        BB_A4; BB_B4; BB_C4; BB_D4; BB_E4; BB_F4; BB_G4; BB_H4;
        BB_A5; BB_B5; BB_C5; BB_D5; BB_E5; BB_F5; BB_G5; BB_H5;
        BB_A6; BB_B6; BB_C6; BB_D6; BB_E6; BB_F6; BB_G6; BB_H6;
        BB_A7; BB_B7; BB_C7; BB_D7; BB_E7; BB_F7; BB_G7; BB_H7;
        BB_A8; BB_B8; BB_C8; BB_D8; BB_E8; BB_F8; BB_G8; BB_H8
        ]
    let BB_CORNERS = BB_A1 ||| BB_H1 ||| BB_A8 ||| BB_H8
    let BB_CENTER = BB_D4 ||| BB_E4 ||| BB_D5 ||| BB_E5
    let BB_LIGHT_SQUARES:Bitboard = 0x55aa55aa55aa55aaUL
    let BB_DARK_SQUARES:Bitboard = 0xaa55aa55aa55aa55UL
    let BB_FILE_A, BB_FILE_B, BB_FILE_C, BB_FILE_D, BB_FILE_E, BB_FILE_F, BB_FILE_G, BB_FILE_H :Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard = 0x0101010101010101UL <<< 0, 0x0101010101010101UL <<< 1, 0x0101010101010101UL <<< 2, 0x0101010101010101UL <<< 3, 0x0101010101010101UL <<< 4, 0x0101010101010101UL <<< 5, 0x0101010101010101UL <<< 6, 0x0101010101010101UL <<< 7 
    let BB_FILES = [BB_FILE_A; BB_FILE_B; BB_FILE_C; BB_FILE_D; BB_FILE_E; BB_FILE_F; BB_FILE_G; BB_FILE_H]
    let BB_RANK_1, BB_RANK_2, BB_RANK_3, BB_RANK_4, BB_RANK_5, BB_RANK_6, BB_RANK_7, BB_RANK_8 :Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard * Bitboard = 0xffUL <<< 8 * 0, 0xffUL <<< 8 * 1, 0xffUL <<< 8 * 2, 0xffUL <<< 8 * 3, 0xffUL <<< 8 * 4, 0xffUL <<< 8 * 5, 0xffUL <<< 8 * 6, 0xffUL <<< 8 * 7 
    let BB_RANKS = [BB_RANK_1; BB_RANK_2; BB_RANK_3; BB_RANK_4; BB_RANK_5; BB_RANK_6; BB_RANK_7; BB_RANK_8]
    let BB_BACKRANKS = BB_RANK_1 ||| BB_RANK_8
    
    let bin (bb:Bitboard) = Convert.ToString(int64(bb), 2)

    let bit_length (bb:Bitboard) =
        let binbb = bin bb
        binbb.Length

    let lsb (bb:Bitboard) =
        ((bb &&& (0UL-bb))|>bit_length) - 1

