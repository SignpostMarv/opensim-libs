#!/bin/bash

TMP_TEST_FILE=$(tempfile)

for t in tests/*lsl; do
    expected_t=${t/\.lsl/_expected.cs}
    mono output.exe $t > $TMP_TEST_FILE
    cmp $TMP_TEST_FILE $expected_t > /dev/null
    DIFF_RESULT=$?

    if [ $DIFF_RESULT -eq 0 ]
    then
        echo "$t OK"
    else
        echo "$t FAIL"
        diff $TMP_TEST_FILE $expected_t
        cp $TMP_TEST_FILE $t.FAIL
    fi

    rm $TMP_TEST_FILE
done
