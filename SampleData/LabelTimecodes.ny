;nyquist plug-in
;version 4
;type analyze
;name "Label Timecodes"
;author "Steve Daulton"
;release 0.0.1
;copyright "GNU General Public License v3"

;; This plug-in is a proof of concept.
;; It will be slow, and has no error checking.

;; Timecode format:
;;
;; 10 samples: 1,-1,1,-1,1,-1,1,-1,1,-1,
;; 27 samples: Sample number
;; 6 samples version
;; 6 samples year
;; 6 samples: month
;; 6 samples: day
;; 6 samples: hour
;; 6 samples: min
;; 6 samples: seconds
;;
;; Total: 79 samples

(defun label-timecodes(sig)
  (let ((labels ()))
    (do ((ar (snd-fetch-array sig 79 1)(snd-fetch-array sig 79 1))
         (count 0 (1+ count)))
        ((not ar) labels)
      (when (match-code ar)
        (push (get-label count ar) labels)))))


(defun match-code (ar)
  ; Written in 'longhand' for speed.
  (and (= (round (aref ar 0)) 1)
       (= (round (aref ar 1)) -1)
       (= (round (aref ar 2)) 1)
       (= (round (aref ar 3)) -1)
       (= (round (aref ar 4)) 1)
       (= (round (aref ar 5)) -1)
       (= (round (aref ar 6)) 1)
       (= (round (aref ar 7)) -1)
       (= (round (aref ar 8)) 1)
       (= (round (aref ar 9)) -1)))


(defun decode (ar)
  (let* ((sp1 (get-digit (aref ar 10) (aref ar 11) (aref ar 12)))
        (sp2 (get-digit (aref ar 13) (aref ar 14) (aref ar 15)))
        (sp3 (get-digit (aref ar 16) (aref ar 17) (aref ar 18)))
        (sp4 (get-digit (aref ar 19) (aref ar 20) (aref ar 21)))
        (sp5 (get-digit (aref ar 22) (aref ar 23) (aref ar 24)))
        (sp6 (get-digit (aref ar 25) (aref ar 26) (aref ar 27)))
        (sp7 (get-digit (aref ar 28) (aref ar 29) (aref ar 30)))
        (sp8 (get-digit (aref ar 31) (aref ar 32) (aref ar 33)))
        (sp9 (get-digit (aref ar 34) (aref ar 35) (aref ar 36)))

        (v1 (get-digit (aref ar 37) (aref ar 38) (aref ar 39)))
        (v2 (get-digit (aref ar 40) (aref ar 41) (aref ar 42)))
        (y1 (get-digit (aref ar 43) (aref ar 44) (aref ar 45)))
        (y2 (get-digit (aref ar 46) (aref ar 47) (aref ar 48)))

        (mn1 (get-digit (aref ar 49) (aref ar 50) (aref ar 51)))
        (mn2 (get-digit (aref ar 52) (aref ar 53) (aref ar 54)))
        (d1 (get-digit (aref ar 55) (aref ar 56) (aref ar 57)))
        (d2 (get-digit (aref ar 58) (aref ar 59) (aref ar 60)))
        (h1 (get-digit (aref ar 61) (aref ar 62) (aref ar 63)))
        (h2 (get-digit (aref ar 64) (aref ar 65) (aref ar 66)))
        (m1 (get-digit (aref ar 67) (aref ar 68) (aref ar 69)))
        (m2 (get-digit (aref ar 70) (aref ar 71) (aref ar 72)))
        (s1 (get-digit (aref ar 73) (aref ar 71) (aref ar 75)))
        (s2 (get-digit (aref ar 76) (aref ar 77) (aref ar 78)))
        (mxxx ( +( + 1 mn1) (* 10 mn2)) )
        )
        
    (format nil " ~a/~a~a/~a~a ~a~a:~a~a:~a~a"
            
            mxxx d2 d1 y2 y1 h2 h1 m2 m1 s2 sp1))
            
            )
  

(defun get-digit (d1 d2 cb)
  (round (/ (+ (round (* d1 10) ) (round (* d1 10) )) 2)))
;(round (/ (+ (round d1) (round d1)) 2)))
  

(defun get-label (sample-num ar)
  ; Return a new label
  (let ((t0 (/ sample-num *sound-srate*)))
    (list t0 t0 (decode ar))))

(label-timecodes *track*)
