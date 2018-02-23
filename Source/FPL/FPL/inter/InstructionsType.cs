namespace FPL.inter
{
    public enum InstructionType
    {
        pushloc,
        pusharg,
        pushfield,
        pushadr,
        pushval,
        pushEAX,

        popadr,
        popEAX,
        pop,

        storeloc,
        storearg,
        storefield,

        add,
        addl,
        adda,
        addf,
        add1,
        addl1,
        adda1,
        addf1,
        sub,
        subl,
        suba,
        subf,
        sub1,
        subl1,
        suba1,
        subf1,
        div,
        divl,
        diva,
        divf,
        mul,
        mull,
        mula,
        mulf,

        jmp,
        call,
        ret,

        eqt,
        eqf,
        let,
        lef,
        mot,
        mof,

        newobjc,
        newobji,
        newobjf,
        newobjs,
        newobjb,

        endP,

        nop,
        endF,
        func,
        @class,
        define,
        classEnd,
        funcEnd,
    }
}
