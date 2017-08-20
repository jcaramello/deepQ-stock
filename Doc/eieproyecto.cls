%archivo de estilo de la clase ``eieproyecto'' (V.M. Alfaro, ver. feb.2013)
% este tiene como base la clase ``memoir''

\NeedsTeXFormat{LaTeX2e}
\ProvidesClass{eieproyecto}[2013/02/08 Informe del Proyecto Eléctrico EIE-UCR]
\LoadClass[letterpaper,11pt,twoside]{memoir}

\DeclareOption*{\PassOptionsToclass{\CurrentOption}{book}}

\newif\ifbtflag
\DeclareOption{borrador}{%
\btflagtrue
}

\ProcessOptions

\RequirePackage[english,spanish,es-noindentfirst,es-nosectiondot,es-nolists]{babel}
\usepackage[utf8]{inputenc}
\RequirePackage[T1]{fontenc}
\RequirePackage{ae,aecompl}
\RequirePackage{amsthm}
\RequirePackage{amsmath}
\RequirePackage{enumitem}
\RequirePackage{graphicx}
\RequirePackage{natbib}
\RequirePackage{url}
\RequirePackage[leftcaption]{sidecap}
\sidecaptionvpos{figure}{t}

\ifbtflag
	%formato para informe en borrador
	\settypeblocksize{20cm}{13cm}{*}
	\setbinding{0cm}
	\setulmargins{*}{*}{1}
	\setlrmargins{*}{*}{1}
	\checkandfixthelayout
	\usepackage{eso-pic}
	\usepackage{color}
	\usepackage{type1cm}
	\makeatletter
	\AddToShipoutPicture{%
		\setlength{\@tempdimb}{0.10\paperwidth}%
		\setlength{\@tempdimc}{0.50\paperheight}%
		\setlength{\unitlength}{1pt}%
		\put(\strip@pt\@tempdimb,\strip@pt\@tempdimc){%
			\makebox(0,0){\rotatebox{90}{\textcolor[gray]{0.80}{\fontsize{2.5cm}{2.5cm}
			\selectfont{BORRADOR}}}}
			}
	}
	\DoubleSpacing
	\renewcommand{\cleardoublepage}{\newpage}
	\openany
\else
	%formato para informe final
	\settypeblocksize{20cm}{13cm}{*}
	\setbinding{1.5cm}
	\setulmargins{*}{*}{1}
	\setlrmargins{*}{*}{1}
	\checkandfixthelayout
\fi

%estilo inicial de las páginas (frontmatther) 
\pagestyle{plain}
\nouppercaseheads
\spanishlcroman

%eie page style (headers)
\makepagestyle{eieheadings}
\makeevenfoot{eieheadings}{}{}{}
\makeoddfoot{eieheadings}{}{}{}
\makeevenhead{eieheadings}{\thepage}{}{\slshape\leftmark}
\makeoddhead{eieheadings}{\slshape\rightmark}{}{\thepage}
\createmark{chapter}{both}{shownumber}{}{ \ }

\renewcommand{\appendixtocname}{Apéndices}
\chapterstyle{tandh}

%definición símbolos matemáticos
\newcommand{\me}{\mathrm{e}}
\newcommand{\mi}{\mathrm{i}}
\newcommand{\mj}{\mathrm{j}}
\newcommand{\md}{\mathrm{d}}

%centrado de figuras y cuadros
\setfloatadjustment{figure}{\centering}
\setfloatadjustment{table}{\centering}

%estilo de la bibliografía (formato APA modificado para español)
\bibliographystyle{apalike_es}

\newcommand{\ucreie}{Universidad de Costa Rica \\ Facultad de Ingeniería \\ Escuela de Ingeniería Eléctrica}
\newcommand{\por}{Por:}
\newcommand{\pie}{Ciudad Universitaria ``Rodrigo Facio'', Costa Rica}
\newcommand{\leyenda}{IE0499 - Proyecto Eléctrico}
\newcommand{\tribunal}{Aprobado por el Tribunal:}
\newcommand{\dircasesor}{Profesor guía}
\newcommand{\memcasesorA}{Profesor lector}
\newcommand{\memcasesorB}{Profesor lector}

\def\dca#1{\gdef\@dca{#1}}
\def\maca#1{\gdef\@maca{#1}}
\def\mbca#1{\gdef\@mbca{#1}}
\def\autor#1{\gdef\@autor{#1}}

%portada
\newcommand{\eietitlepage}{
\thispagestyle{empty}
\begin{center}
	\Large\bfseries\ucreie \\
	\vskip 5em
	\LARGE\bfseries\@title \\
	\vskip 3em
	\large\bfseries\por
	\vskip 1em
	\large\bfseries\@autor \\
	\vfill
	\large\bfseries\pie
	\vskip 1em 
	\large\bfseries\@date
\end{center}}

%hoja de aprobación
\newcommand{\eieaprovalpage}{
\newpage
\thispagestyle{empty}
}

\newcommand{\autorcap}[1]{
\begin{flushright}
	\Large{\textbf{\emph{#1}}}
\end{flushright}}

\endinput
