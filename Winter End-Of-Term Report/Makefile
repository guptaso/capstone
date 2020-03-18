# this is a comment

TARGET=main


default: pdf

dvi: ${TARGET}.tex 
#	run latex twice to get references correct
	latex ${TARGET}.tex
#	you can also have a bibtex line here
	latex $(TARGET).tex
	latex $(TARGET).tex
	latex $(TARGET).tex
	latex $(TARGET).tex
#ps: dvi
#	dvips -R -Poutline -t letter ${TARGET}.dvi -o ${TARGET}.ps

#pdf: ps
#	ps2pdf ${TARGET}.ps

pdf: 
	pdflatex $(TARGET).tex
	pdflatex $(TARGET).tex
	pdflatex $(TARGET).tex

clean-dvi:
	rm -f ${TARGET}.aux
	rm -f ${TARGET}.log
	rm -f ${TARGET}.dvi
	rm -f ${TARGET}.out
	rm -f ${TARGET}.toc

clean-ps: clean-dvi
	rm -f ${TARGET}.ps

clean-pdf: clean-ps
	rm -f ${TARGET}.bbl
	rm -f ${TARGET}.blg
	rm -f ${TARGET}.pdf


clean: clean-pdf
