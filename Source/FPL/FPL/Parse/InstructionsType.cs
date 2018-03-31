namespace FPL.Parse
{
    public enum InstructionType
    {
        pushloc,
        pusharg,
        pushfield,
        pushval,
        pushsta,
        //pushEAX,

        pop,
        //popEAX,

        storeloc,
        storearg,
        storefield,
        storesta,

        add_i,
        add_f,
        add_c,
        sub_i,
        sub_f,
        sub_c,
        div_i,
        div_f,
        div_c,
        mul_i,
        mul_f,
        mul_c,

        //优化后会使用的指令
        addl,
        adda,
        addf,
        add1,
        addl1,
        adda1,
        addf1,
        subl,
        suba,
        subf,
        sub1,
        subl1,
        suba1,
        subf1,
        divl,
        diva,
        divf,
        mull,
        mula,
        mulf,

        jmp,
        call,
        ret,

        eq,
        le,
        mo,
        jt,
        jf,

        newobjc,

        endP,

        nop,
        endF,
        func,
        @class,
        define,
        classEnd,
        funcEnd
    }
}